using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChurchTrigger : MonoBehaviour
{
    public float countdownTime = 10f; // 10 saniye
    public Text countdownText;        // UI Text (Inspector'dan sürükle)

    private bool playerInside = false;
    private float timer;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            timer = countdownTime;
            StartCoroutine(StartCountdown());
            Debug.Log("⛪ Kilise alanına girildi! Sayaç başladı: " + countdownTime + " saniye");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            StopAllCoroutines();
            if (countdownText != null) countdownText.text = "";
            Debug.Log("🚶 Kilise alanından çıkıldı. Sayaç durduruldu.");
        }
    }

    IEnumerator StartCountdown()
    {
        while (playerInside && timer > 0)
        {
            timer -= Time.deltaTime;

            if (countdownText != null)
                countdownText.text = Mathf.Ceil(timer).ToString() + " saniye...";

            yield return null;
        }

        if (playerInside) // Hâlâ içerideyse (çıkmadıysa)
        {
            Debug.Log("⏰ Sayaç bitti! Okul'a geçiliyor...");
            if (GameManager.Instance != null)
                GameManager.Instance.GoToSchool();
        }
    }
}