using UnityEngine;

public class NoiseEmitter : MonoBehaviour
{
    public float walkNoise = 4f, runNoise = 8f;
    public bool isRunning;
    void Update()
    {
        float r = isRunning ? runNoise : walkNoise;
        Collider[] c = Physics.OverlapSphere(transform.position, r, LayerMask.GetMask("Enemy"));
        foreach (var col in c)
        {
            var g = col.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
            if (g) g.destination = transform.position; // ст€гиваем патрулей на шум
        }
    }
}