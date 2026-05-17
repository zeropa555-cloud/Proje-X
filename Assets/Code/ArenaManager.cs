using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        public List<GameObject> enemyPrefabs;
        public List<Transform> spawnPoints;
    }

    public List<Wave> waves = new List<Wave>();

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private bool waveActive = false;

    void Start()
    {
        if (GameManager.Instance != null)
            currentWaveIndex = GameManager.Instance.currentWave;

        StartNextWave();
    }

    void StartNextWave()
    {
        int maxWaves = (GameManager.Instance != null && GameManager.Instance.currentChapter == 2) ? 2 : 1;

        if (currentWaveIndex >= maxWaves || currentWaveIndex >= waves.Count)
        {
            Debug.Log("🏆 Tüm düşmanlar öldü! 2 saniye sonra Orman'a geçiliyor...");
            Invoke(nameof(GoToForest), 2f);
            return;
        }

        waveActive = true;
        Wave currentWave = waves[currentWaveIndex];
        enemiesAlive = currentWave.enemyPrefabs.Count;

        for (int i = 0; i < currentWave.enemyPrefabs.Count; i++)
        {
            if (currentWave.enemyPrefabs[i] == null) continue;

            Vector3 spawnPos = (i < currentWave.spawnPoints.Count && currentWave.spawnPoints[i] != null)
                ? currentWave.spawnPoints[i].position
                : transform.position;

            GameObject enemy = Instantiate(currentWave.enemyPrefabs[i], spawnPos, Quaternion.identity);

            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(spawnPos, out hit, 5f, NavMesh.AllAreas))
                    enemy.transform.position = hit.position;
            }
        }

        Debug.Log("⚔️ " + currentWave.waveName + " başladı! Düşman: " + enemiesAlive);
    }

    public void OnEnemyDied(GameObject enemy)
    {
        if (!waveActive) return;

        enemiesAlive--;
        Debug.Log("💀 Düşman öldü. Kalan: " + enemiesAlive);

        if (enemiesAlive <= 0)
            WaveCleared();
    }

    void WaveCleared()
    {
        waveActive = false;
        currentWaveIndex++;

        if (GameManager.Instance != null)
            GameManager.Instance.currentWave = currentWaveIndex;

        Invoke(nameof(StartNextWave), 3f);
    }

    void GoToForest()
    {
        Debug.Log("🌲 GoToForest() ÇALIŞTI!"); // ⬅️ BU SATIR EKLENDİ
        if (GameManager.Instance != null)
            GameManager.Instance.ArenaCleared();
        else
            Debug.LogError("❌ GameManager NULL! Geçiş yapılamıyor.");
    }
}