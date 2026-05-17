using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GoToNextLevel();
        }
    }
}