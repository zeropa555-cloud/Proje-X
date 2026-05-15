using UnityEngine;

public class MouseLook : MonoBehaviour
{
    // [Header] Inspector'da güzel görünmesi için
    [Header("Mouse Ayarlarý")]
    public float mouseSensitivity = 400f;

    // Bu alaný Inspector'dan dolduracaðýz
    public Transform playerBody;

    // Dikey bakýþ açýsýný hafýzada tutar (baþý arkaya atamazsýn)
    private float xRotation = 0f;

    void Start()
    {
        // Oyun baþlayýnca mouse'u ekranýn ortasýna kilitler ve gizler
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouse inputunu al (Unity otomatik verir)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ---------------------------------------------------------
        // 1. DÝKEY BAKIÞ (Yukarý / Aþaðý)
        // ---------------------------------------------------------
        // xRotation'dan mouseY'yi çýkarýyoruz (tersine çevirme mantýðý)
        xRotation -= mouseY;

        // Baþý -90 derece (yere bak) ile +90 derece (gökyüzüne bak) arasýna sýnýrla
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // CameraHolder'ý X ekseninde döndür (local = kendi ekseninde)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ---------------------------------------------------------
        // 2. YATAY BAKIÞ (Saða / Sola)
        // ---------------------------------------------------------
        // Player gövdesini Y ekseninde (dikey eksen) döndür
        playerBody.Rotate(Vector3.up * mouseX);
    }
}