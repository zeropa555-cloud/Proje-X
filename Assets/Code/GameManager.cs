using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int currentChapter = 1;
    public int currentWave = 0;

    public string homeScene = "Ev";
    public string arenaScene = "Arena";
    public string forestScene = "orman";
    public string schoolScene = "Okul";

    public int playerHealth = 100;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GoToArena()
    {
        StartCoroutine(TransitionTo(arenaScene));
    }

    public void ArenaCleared()
    {
        StartCoroutine(TransitionTo(forestScene));
    }

    public void GoToSchool()
    {
        StartCoroutine(TransitionTo(schoolScene));
    }

    // ⬇️ GÜNCELLENDİ: Chapter 2'de EV'e dön, farklı noktadan başla!
    public void GoToChapter2()
    {
        Debug.Log("🎉 TEBRİKLER! Chapter 2 başlıyor... Ev'in yeni noktasına gidiliyor!");
        currentChapter = 2;
        currentWave = 0;
        playerHealth = 100;
        StartCoroutine(TransitionTo(homeScene)); // ⬅️ Artık Ev'e dönüyor!
    }

    public void ResetChapter()
    {
        Debug.Log("🔄 Reset! Chapter 1 başa sarılıyor...");
        playerHealth = 100;
        currentChapter = 1;
        currentWave = 0;
        StartCoroutine(TransitionTo(homeScene));
    }

    IEnumerator TransitionTo(string sceneName)
    {
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeOut();

        SceneManager.LoadScene(sceneName);
        yield return null;

        // Player'ı bul
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.SetHealth(playerHealth);
                Debug.Log("❤️ Can yenilendi: " + playerHealth);
            }

            // ⬇️ YENİ: Chapter'a göre farklı spawn noktası kullan!
            GameObject spawn = null;

            if (currentChapter == 2)
            {
                // Chapter 2'de farklı spawn noktası ara
                spawn = GameObject.FindGameObjectWithTag("SpawnPoint_Chapter2");
                Debug.Log("📍 Chapter 2 spawn noktası kullanılıyor...");
            }

            // Chapter 2 spawn yoksa veya Chapter 1'deyse normal spawn kullan
            if (spawn == null)
            {
                spawn = GameObject.FindGameObjectWithTag("SpawnPoint");
                Debug.Log("📍 Normal spawn noktası kullanılıyor...");
            }

            if (spawn != null)
                player.transform.position = spawn.transform.position;
        }

        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeIn();
    }
}