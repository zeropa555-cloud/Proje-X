using UnityEngine;

public class DoorPortal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🚪 Kapıdan çıkıldı, Arena'ya gidiliyor...");
            GameManager.Instance.GoToArena();
        }
    }
}