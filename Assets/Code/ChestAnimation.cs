using UnityEngine;

public class SandikKontrol : MonoBehaviour
{
    public bool acik Mi = false;
    public float acilmaAcisi = -90f; // Kapağın ne kadar açılacağı
    public float hiz = 2f;

    private Quaternion kapaliRotasyon;
    private Quaternion acikRotasyon;

    void Start()
    {
        kapaliRotasyon = transform.localRotation;
        // Kapağın hangi eksende açılacağına göre (X, Y veya Z) Euler değerini değiştir
        acikRotasyon = Quaternion.Euler(acilmaAcisi, 0, 0) * kapaliRotasyon;
    }

    void Update()
    {
        if (acikMi)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, acikRotasyon, Time.deltaTime * hiz);
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, kapaliRotasyon, Time.deltaTime * hiz);
        }
    }

    public void SandikTetikle()
    {
        acikMi = !acikMi;
    }
}