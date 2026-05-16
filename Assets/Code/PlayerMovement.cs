using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket")]
    public float moveSpeed = 6f;

    [Header("Zıplama")]
    public float jumpForce = 8f;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Fizikle devrilmesin
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Takılma olmaz
    }

    void Update()
    {
        // Zıplama
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Önceki y hızını sıfırla
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Yer kontrolü
        CheckGround();

        // WASD input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Karakterin baktığı yöne göre hareket (FPSLook zaten döndürüyor)
        Vector3 move = (transform.right * h + transform.forward * v).normalized;

        // Hız uygula (sadece x/z, y yerçekimi/zıplama)
        Vector3 targetVelocity = move * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;
    }

    void CheckGround()
    {
        // Ayaklarından ışın at
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.05f, groundMask);

        // Layer atanmamışsa her şeyi zemin say
        if (groundMask == 0)
            isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.05f);
    }

    // Editörde zemini kontrol etmek için çizgi
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.05f;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance + 0.05f));
    }
}