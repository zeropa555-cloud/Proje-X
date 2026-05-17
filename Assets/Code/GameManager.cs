using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int playerHealth = 100;
    public string[] levelNames = { "Level1", "Level2" };
    public int currentLevel = 0;

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

    public void GoToNextLevel()
    {
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        // Canı kaydet
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) playerHealth = ph.GetCurrentHealth();
        }

        // Gözün kararır
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeOut();

        // Sahne değiştir
        currentLevel++;
        if (currentLevel >= levelNames.Length) currentLevel = 0;
        SceneManager.LoadScene(levelNames[currentLevel]);
        yield return null;

        // Yeni sahnede player'ı bul ve yerleştir
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.SetHealth(playerHealth);

            GameObject spawn = GameObject.FindGameObjectWithTag("SpawnPoint");
            if (spawn != null) player.transform.position = spawn.transform.position;
        }

        // Gözün açılır
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeIn();
    }
}