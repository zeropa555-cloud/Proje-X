using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarlar�")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;

    [Header("Z�plama Ayarlar�")]
    public float jumpForce = 7f;
    public LayerMask groundLayer; // Zemin layer'�

    [Header("Yer Kontrol�")]
    public Transform groundCheck; // Ayaklar�n alt�na bo� bir GameObject koy
    public float groundDistance = 0.4f;

    private Rigidbody rb;
    private bool isGrounded;
    private float horizontal;
    private float vertical;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Devrilmesin diye
    }

    void Update()
    {
        // Yer kontrol�
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        // Girdi al
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // Z�plama
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Hareket y�n� (kameraya g�re)
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Kameran�n bakt��� y�ne g�re hareket
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Ko�ma kontrol�
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

            // H�z uygula (yukar�-a�a�� h�z�n� koru, sadece yatayda hareket et)
            Vector3 targetVelocity = moveDir * currentSpeed;
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // Durunca yava��a dur (iste�e ba�l�, an�nda durmas�n� istersen bu blo�u sil)
            Vector3 stopVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, stopVelocity, 0.2f);
        }
    }
}