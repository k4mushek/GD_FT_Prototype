using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExtractionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        MissionManager.Instance?.ReachedExtraction();
        // здесь можно загрузить сцену базы, показать экран результатов и т.п.
    }
}