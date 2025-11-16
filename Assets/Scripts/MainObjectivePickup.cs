using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MainObjectivePickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // забрали цель
        MissionManager.Instance?.CompleteMainObjective();

        // визуально отключаем объект
        gameObject.SetActive(false);
    }
}