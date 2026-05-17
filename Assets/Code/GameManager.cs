using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int playerHealth = 100;
    public string[] levelNames = { "Level1", "Level2", "Level3", "Level4" };
    public bool[] levelCompleted = { false, false, false, false };
    public int currentLevel = 0;

    private const string ForestSceneName = "orman";
    private const string CollesiumSceneName = "Collesium";

    private struct SceneState
    {
        public bool hasPosition;
        public Vector3 position;
        public bool hasHealth;
        public int health;
    }

    private readonly Dictionary<string, SceneState> sceneStates = new Dictionary<string, SceneState>();

    private bool isTransitioning;
    private GameObject oldPersistentPlayer; // Eski persistent playeri takip et
    private GameObject persistentPlayer; // Şu anki persistent player

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

        string currentSceneName = SceneManager.GetActiveScene().name;
        CaptureSceneState(currentSceneName);

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

        string nextSceneName = levelNames[currentLevel];

        // Mevcut persistent playeri kaydet
        oldPersistentPlayer = GameObject.FindGameObjectWithTag("Player");

        // Sahne değiştir
        SceneManager.LoadScene(nextSceneName);
        yield return null;

        // Yeni sahnede bir Player varsa, eski persistent playeri yok et
        HandlePlayerTransition(oldPersistentPlayer);

        // Pozisyon restore edilir
        RestoreSceneState(nextSceneName);

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

    private void CaptureSceneState(string sceneName)
    {
        if (!ShouldTrackScene(sceneName))
            return;

        SceneState sceneState = GetSceneState(sceneName);

        if (ShouldSavePosition(sceneName) && TryGetPlayerTransform(out Transform playerTransform))
        {
            sceneState.hasPosition = true;
            sceneState.position = playerTransform.position;
            Debug.Log($"[GameManager] KAYDET: {sceneName} - Pozisyon kaydedildi: {sceneState.position}");
        }

        if (ShouldSaveHealth(sceneName) && TryGetPlayerHealth(out PlayerHealth playerHealthComponent))
        {
            sceneState.hasHealth = true;
            sceneState.health = playerHealthComponent.GetCurrentHealth();
            playerHealth = sceneState.health;
            Debug.Log($"[GameManager] KAYDET: {sceneName} - Sağlık kaydedildi: {sceneState.health}");
        }

        sceneStates[sceneName] = sceneState;
    }

    private void RestoreSceneState(string sceneName)
    {
        if (!ShouldTrackScene(sceneName))
            return;

        if (!sceneStates.TryGetValue(sceneName, out SceneState sceneState))
        {
            Debug.LogWarning($"[GameManager] RESTORE: {sceneName} için state bulunamadı!");
            return;
        }

        if (ShouldLoadPosition(sceneName) && sceneState.hasPosition && TryGetPlayerTransform(out Transform playerTransform))
        {
            ApplyPosition(playerTransform, sceneState.position);
            Debug.Log($"[GameManager] RESTORE: {sceneName} - Pozisyon restore edildi: {sceneState.position}");
        }

        if (ShouldLoadHealth(sceneName) && sceneState.hasHealth && TryGetPlayerHealth(out PlayerHealth playerHealthComponent))
        {
            playerHealthComponent.SetHealth(sceneState.health);
            playerHealth = sceneState.health;
            Debug.Log($"[GameManager] RESTORE: {sceneName} - Sağlık restore edildi: {sceneState.health}");
        }
    }

    private void HandlePlayerTransition(GameObject oldPersistentPlayer)
    {
        // Eski persistent player'ı yok et
        if (oldPersistentPlayer != null)
        {
            Debug.Log($"Eski persistent player yok edildi: {oldPersistentPlayer.name}");
            Destroy(oldPersistentPlayer);
        }
        
        // Yeni sahnedeki Player'ı bul
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject newScenePlayer = null;
        
        foreach (GameObject rootObj in activeScene.GetRootGameObjects())
        {
            if (rootObj.CompareTag("Player"))
            {
                newScenePlayer = rootObj;
                break;
            }
        }
        
        if (newScenePlayer != null)
        {
            DontDestroyOnLoad(newScenePlayer);
            persistentPlayer = newScenePlayer;
            Debug.Log($"Yeni player persistent yapıldı: {newScenePlayer.name}");
        }
        else
        {
            Debug.LogWarning("Yeni sahnede player bulunamadı!");
            persistentPlayer = null;
        }
    }

    private SceneState GetSceneState(string sceneName)
    {
        if (sceneStates.TryGetValue(sceneName, out SceneState sceneState))
            return sceneState;

        return new SceneState();
    }

    private bool ShouldTrackScene(string sceneName)
    {
        return ShouldSavePosition(sceneName) || ShouldSaveHealth(sceneName) || ShouldLoadPosition(sceneName) || ShouldLoadHealth(sceneName);
    }

    private bool ShouldSavePosition(string sceneName)
    {
        // TÜM sahnelerde pozisyon kaydedilsin
        return true;
    }

    private bool ShouldLoadPosition(string sceneName)
    {
        // TÜM sahnelerde pozisyon restore edilsin
        return true;
    }

    private bool ShouldSaveHealth(string sceneName)
    {
        return sceneName == CollesiumSceneName;
    }

    private bool ShouldLoadHealth(string sceneName)
    {
        return sceneName == CollesiumSceneName;
    }

    private bool TryGetPlayerTransform(out Transform playerTransform)
    {
        if (persistentPlayer != null)
        {
            playerTransform = persistentPlayer.transform;
            return true;
        }

        // Fallback: FindGameObjectWithTag kullan
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            return true;
        }

        playerTransform = null;
        return false;
    }

    private bool TryGetPlayerHealth(out PlayerHealth playerHealthComponent)
    {
        playerHealthComponent = null;

        if (GameObject.FindGameObjectWithTag("Player") is GameObject playerObject)
        {
            playerHealthComponent = playerObject.GetComponent<PlayerHealth>();
        }

        return playerHealthComponent != null;
    }

    private void ApplyPosition(Transform playerTransform, Vector3 position)
    {
        Rigidbody rigidbody = playerTransform.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            rigidbody.position = position;
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            playerTransform.position = position;
        }
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