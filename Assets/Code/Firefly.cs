using System.Collections;
using UnityEngine;

public class AtesBocegi : MonoBehaviour
{
    private Light atesIsigi;

    [Header("Zamanlama Ayarları")]
    [Tooltip("Işığın her seferinde ne kadar süre açık kalacağı")]
    public float acikKalmaSuresi = 0.15f; 
    
    [Tooltip("Peş peşe yanarken aradaki kapalı kalma süresi")]
    public float kapaliKalmaSuresi = 0.15f; 

    [Tooltip("3 kez yanıp söndükten sonraki büyük bekleme süresi")]
    public float uzunBeklemeSuresi = 2.5f; 

    void Start()
    {
        atesIsigi = GetComponent<Light>();

        if (atesIsigi != null)
        {
            StartCoroutine(AtesBocegiRitmi());
        }
        else
        {
            Debug.LogError("Lütfen bu kodu içinde Light (Işık) olan bir objeye atayın!");
        }
    }

    IEnumerator AtesBocegiRitmi()
    {
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                atesIsigi.enabled = true;  
                yield return new WaitForSeconds(acikKalmaSuresi);

                atesIsigi.enabled = false; 
                yield return new WaitForSeconds(kapaliKalmaSuresi);
            }

            yield return new WaitForSeconds(uzunBeklemeSuresi);
        }
    }
}