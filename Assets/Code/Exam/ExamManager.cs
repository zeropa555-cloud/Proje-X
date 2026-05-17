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
    public Text resultText; // ⬅️ Sonuç mesajı için (isteğe bağlı)

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

        // ⬇️ YENİ: Doğru/Yanlış kontrolü
        if (dogruSayisi == toplamSoru && toplamSoru > 0)
        {
            // TÜM SORULAR DOĞRU → Chapter 2
            Debug.Log("🎉 Tüm sorular doğru! Chapter 2'ye geçiliyor...");
            if (resultText != null) resultText.text = "Tebrikler! Geçtiniz...";

            Invoke(nameof(GoToChapter2), 2f); // 2 saniye bekle, sonra geç
        }
        else
        {
            // YANLIŞ VAR → Chapter 1 başa sar
            Debug.Log("❌ Yanlış cevap! Baştan başlanıyor...");
            if (resultText != null) resultText.text = "Başarısız! Baştan başlanıyor...";

            Invoke(nameof(ResetToHome), 2f); // 2 saniye bekle, sonra reset
        }
    }

    void GoToChapter2()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GoToChapter2();

        SınavıKapat();
    }

    void ResetToHome()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResetChapter();

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