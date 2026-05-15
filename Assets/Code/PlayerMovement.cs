using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 5f;
    public float crouchSpeed = 2.5f;

    [Header("Sürünme Ayarlarý")]
    public KeyCode crouchKey = KeyCode.LeftControl;
    public float crouchCameraY = 0.5f;
    public float crouchColliderHeight = 0.5f;
    public float transitionSpeed = 10f;

    private Rigidbody rb;
    private CapsuleCollider col;
    private Transform cameraHolder;

    private Vector3 moveDirection;
    private float currentSpeed;

    // Baþlangýç deðerleri
    private float normalColliderHeight;
    private float normalColliderCenterY;
    private float normalCameraY;
    private bool isCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        cameraHolder = transform.Find("CameraHolder");

        // Baþlangýç deðerlerini kaydet
        normalColliderHeight = col.height;
        normalColliderCenterY = col.center.y;
        currentSpeed = moveSpeed;

        if (cameraHolder != null)
            normalCameraY = cameraHolder.localPosition.y;

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // --- SÜRÜNME: Ctrl BASILI TUTUNCA sürün, BIRAKINCA kalk ---
        // GetKey = basýlý tutunca, GetKeyDown = tek basýþta toggle
        isCrouching = Input.GetKey(crouchKey);

        // Hedef deðerler
        float targetHeight = isCrouching ? crouchColliderHeight : normalColliderHeight;
        float targetCenterY = isCrouching ? (crouchColliderHeight / 2f) : normalColliderCenterY;
        float targetCamY = isCrouching ? crouchCameraY : normalCameraY;
        currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        // --- COLLIDER: Anlýk deðiþtir (fizik patlamasýn diye Lerp YOK) ---
        // Sürünme/kalkma anýnda TEK SEFER deðiþtir, her frame deðil
        if (Mathf.Abs(col.height - targetHeight) > 0.01f)
        {
            col.height = targetHeight;
            col.center = new Vector3(0, targetCenterY, 0);
        }

        // --- KAMERA: Yumuþak in/kalk ---
        if (cameraHolder != null)
        {
            Vector3 camPos = cameraHolder.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetCamY, transitionSpeed * Time.deltaTime);
            cameraHolder.localPosition = camPos;
        }

        // --- HAREKET INPUT ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = (transform.right * h + transform.forward * v).normalized;
    }

    void FixedUpdate()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 target = rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(target);
        }
    }
}