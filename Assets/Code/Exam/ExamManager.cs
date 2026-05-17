using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExamManager : MonoBehaviour
{
    [System.Serializable]
    public class SoruVerisi
    {
        public string soruAdi = "Soru X"; 
        public Toggle toggleA;
        public Toggle toggleB;
        public Toggle toggleC;
        
        [Tooltip("Doğru şıkkı BÜYÜK harfle yazın: A, B veya C")]
        public string dogruCevap; 
    }

    [Header("Sınav Dosyaları")]
    public List<SoruVerisi> sorular = new List<SoruVerisi>();

    [Header("Arayüz Ayarları")]
    public GameObject examPaperPanel; 

   
    public void SinaviBitir()
    {
        int toplamSoru = sorular.Count;
        int dogruSayisi = 0;
        int yanlisSayisi = 0;
        int bosSayisi = 0;

        for (int i = 0; i < sorular.Count; i++)
        {
            SoruVerisi soru = sorular[i];
            string secilenSik = "";

             
            if (soru.toggleA != null && soru.toggleA.isOn) secilenSik = "A";
            else if (soru.toggleA != null && soru.toggleA.isOn) secilenSik = "A";
            else if (soru.toggleB != null && soru.toggleB.isOn) secilenSik = "B";
            else if (soru.toggleC != null && soru.toggleC.isOn) secilenSik = "C";

            
            if (string.IsNullOrEmpty(secilenSik))
            {
                bosSayisi++;
            }
            else if (secilenSik == soru.dogruCevap.ToUpper())
            {
                dogruSayisi++;
            }
            else
            {
                yanlisSayisi++;
            }
        }

         
        Debug.Log($"Sınav Sonucu -> Doğru: {dogruSayisi} | Yanlış: {yanlisSayisi} | Boş: {bosSayisi} / Toplam: {toplamSoru}");

        
        SınavıKapat();
    }

    private void SınavıKapat()
    {
        if (examPaperPanel != null)
        {
            examPaperPanel.SetActive(false); 
            Cursor.lockState = CursorLockMode.Locked;  
            Cursor.visible = false;  
        }
    }
}