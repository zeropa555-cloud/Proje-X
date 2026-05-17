using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float countdownTime = 60f; // 1 dakika = 60 saniye
    private float remainingTime;
    private bool isRunning;

    void Start()
    {
        remainingTime = countdownTime;
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning)
            return;

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
        // Sahne bittiğini işaretle
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToNextLevel();
        }
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public bool IsTimerRunning()
    {
        return isRunning;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        remainingTime = countdownTime;
        isRunning = true;
    }
}
