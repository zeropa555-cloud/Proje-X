using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float countdownTime = 60f;
    private float remainingTime;
    private bool isRunning;

    void Start()
    {
        remainingTime = countdownTime;
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            isRunning = false;
            OnTimerFinished();
        }
    }

    private void OnTimerFinished()
    {
        Debug.Log("⏰ Süre doldu!");

        // ⬇️ ŞİMDİLİK KAPALI - İleride açılacak
        // if (GameManager.Instance != null)
        //     GameManager.Instance.GoToNextLevel();
    }

    public float GetRemainingTime() => remainingTime;
    public bool IsTimerRunning() => isRunning;
    public void StopTimer() => isRunning = false;
    public void ResumeTimer() => isRunning = true;

    public void ResetTimer()
    {
        remainingTime = countdownTime;
        isRunning = true;
    }
}