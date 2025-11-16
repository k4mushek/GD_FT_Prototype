using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OptionalObjectiveZone : MonoBehaviour
{
    [Tooltip("true - сразу считаем, что выживший спасён, false - только открываем задачу")]
    public bool completeOnEnter = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (completeOnEnter)
            MissionManager.Instance?.CompleteOptional();
        else
            MissionManager.Instance?.DiscoverOptional();
    }
}