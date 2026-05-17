using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Oyuncu Verileri")]
    public int playerHealth = 100;
    public int playerScore = 0;

    [Header("Sahne Sistemi")]
    public int currentLevel = 0;
    public string[] levelNames = { "Level1", "Level2", "Level3", "Level4" };

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

    // Bir sonraki sahneye geç (Fade'li!)
    public void GoToNextLevel()
    {
        StartCoroutine(TransitionToNextLevel());
    }

    IEnumerator TransitionToNextLevel()
    {
        // 1. Önce canı kaydet
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) playerHealth = ph.GetCurrentHealth();
        }

        // 2. Ekran kararsın (Fade Out) - "Gözümüz karalır"
        if (FadeManager.Instance != null)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeOut());
        }

        // 3. Sıradaki sahneye geç
        currentLevel++;
        if (currentLevel >= levelNames.Length)
        {
            currentLevel = 0;
        }

        SceneManager.LoadScene(levelNames[currentLevel]);

        // 4. Sahne yüklendikten sonra 1 frame bekle
        yield return null;

        // 5. Oyuncuyu yerleştir
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpawnPlayer(player);
        }

        // 6. Ekran açılsın (Fade In) - "Gözümüz açılır"
        if (FadeManager.Instance != null)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeIn());
        }
    }

    public void SpawnPlayer(GameObject player)
    {
        if (player == null) return;

        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.SetHealth(playerHealth);
        }

        GameObject spawn = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawn != null)
        {
            player.transform.position = spawn.transform.position;
        }
    }
}