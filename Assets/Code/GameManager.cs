using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int playerHealth = 100;
    public string[] levelNames = { "Level1", "Level2", "Level3", "Level4" };
    public bool[] levelCompleted = { false, false, false, false };
    public int currentLevel = 0;

    private bool isTransitioning;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentLevel = GetSceneIndex(SceneManager.GetActiveScene().name);
        SyncCompletionArray();
    }

    public void GoToNextLevel()
    {
        if (isTransitioning)
            return;

        StartCoroutine(ChangeScene());
    }

    public void MarkCurrentSceneCompleted()
    {
        SetSceneCompleted(currentLevel, true);
    }

    public void TriggerCurrentSceneCompleted()
    {
        MarkCurrentSceneCompleted();
    }

    public void SetSceneCompleted(int sceneIndex, bool completed)
    {
        SyncCompletionArray();

        if (sceneIndex < 0 || sceneIndex >= levelCompleted.Length)
            return;

        levelCompleted[sceneIndex] = completed;
    }

    public bool IsSceneCompleted(int sceneIndex)
    {
        SyncCompletionArray();

        if (sceneIndex < 0 || sceneIndex >= levelCompleted.Length)
            return false;

        return levelCompleted[sceneIndex];
    }

    IEnumerator ChangeScene()
    {
        isTransitioning = true;

        // Canı kaydet
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //if (player != null)
        //{
        //    PlayerHealth ph = player.GetComponent<PlayerHealth>();
        //    if (ph != null) playerHealth = ph.GetCurrentHealth();
        //}

        // Gözün kararır
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeOut();

        // Bir sonraki oynanabilir sahneyi bul
        currentLevel = GetNextPlayableSceneIndex(currentLevel);

        if (levelNames == null || levelNames.Length == 0 || currentLevel < 0 || currentLevel >= levelNames.Length || string.IsNullOrWhiteSpace(levelNames[currentLevel]))
        {
            Debug.LogError("GameManager: Yüklenecek sahne adı boş veya geçersiz.");
            isTransitioning = false;
            yield break;
        }

        // Sahne değiştir
        SceneManager.LoadScene(levelNames[currentLevel]);
        yield return null;

        // Yeni sahnede player'ı bul ve yerleştir

        // Gözün açılır
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.FadeIn();

        isTransitioning = false;
    }

    private int GetNextPlayableSceneIndex(int fromIndex)
    {
        SyncCompletionArray();

        if (levelNames == null || levelNames.Length == 0)
            return 0;

        int sceneCount = levelNames.Length;

        for (int offset = 1; offset <= sceneCount; offset++)
        {
            int nextIndex = (fromIndex + offset) % sceneCount;

            if (!string.IsNullOrWhiteSpace(levelNames[nextIndex]) && !levelCompleted[nextIndex])
                return nextIndex;
        }

        // Tüm sahneler tamamlandıysa döngüyü bozmamak için normal sıradaki sahneye dön.
        return (fromIndex + 1) % sceneCount;
    }

    private int GetSceneIndex(string sceneName)
    {
        if (levelNames == null)
            return 0;

        for (int i = 0; i < levelNames.Length; i++)
        {
            if (levelNames[i] == sceneName)
                return i;
        }

        return 0;
    }

    private void SyncCompletionArray()
    {
        if (levelNames == null)
            levelNames = new string[0];

        if (levelCompleted == null || levelCompleted.Length != levelNames.Length)
        {
            bool[] newCompleted = new bool[levelNames.Length];

            if (levelCompleted != null)
            {
                int count = Mathf.Min(levelCompleted.Length, newCompleted.Length);
                for (int i = 0; i < count; i++)
                {
                    newCompleted[i] = levelCompleted[i];
                }
            }

            levelCompleted = newCompleted;
        }
    }
}