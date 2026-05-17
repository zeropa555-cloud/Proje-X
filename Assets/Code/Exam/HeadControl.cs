using UnityEngine;

public class HeadControl : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float minYaw = -180f;
    [SerializeField] private float maxYaw = 180f;
    [SerializeField] private float minPitch = -60f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private LayerMask paperLayer;

    private float yaw;
    private float pitch;
    private bool isUIOpen = false;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main != null ? Camera.main.transform : null;
        }

        Vector3 playerRotation = transform.localEulerAngles;
        yaw = NormalizeAngle(playerRotation.y);

        if (cameraTransform != null)
        {
            pitch = NormalizeAngle(cameraTransform.localEulerAngles.x);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (cameraTransform == null)
        {
            return;
        }
        // If UI is open, freeze camera and only allow ESC to close it
        if (isUIOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUI();
            }

            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Handle paper layer click
        if (Input.GetMouseButtonDown(0))
        {
            TryToInteractPaper();
        }

        // Handle ESC to close UI (redundant when UI closed, but kept for responsiveness)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
    }

    private void TryToInteractPaper(){
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000f, paperLayer)){
            Debug.Log($"hit {hit.collider.gameObject.name}");
            OpenUI();
        }
    }

    private void OpenUI(){
        if(uiPanel != null){
            uiPanel.SetActive(true);
            isUIOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void CloseUI(){
        if(uiPanel != null && isUIOpen){
            uiPanel.SetActive(false);
            isUIOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private static float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}
