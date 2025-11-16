using UnityEngine;

public class RoofArea : MonoBehaviour
{
    public static bool PlayerOnRoof { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerOnRoof = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        PlayerOnRoof = false;
    }
}