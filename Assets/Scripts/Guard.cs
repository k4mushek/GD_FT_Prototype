using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    public Transform[] points;
    public float viewAngle = 60f;
    public float viewDistance = 12f;
    public LayerMask obstacleMask;
    public Transform eye;
    public Transform player;
    int i;
    NavMeshAgent agent;
    float loseTimer;

    void Awake() { agent = GetComponent<NavMeshAgent>(); agent.autoBraking = false; Next(); }
    void Next() { if (points.Length == 0) return; agent.destination = points[i].position; i = (i + 1) % points.Length; }

    void Update()
    {
        // патруль
        if (!agent.pathPending && agent.remainingDistance < 0.5f) Next();

        // простое зрение по углу/дистанции/преградам
        Vector3 dir = (player.position - eye.position);
        if (dir.magnitude <= viewDistance && Vector3.Angle(eye.forward, dir) < viewAngle)
        {
            if (!Physics.Raycast(eye.position, dir.normalized, dir.magnitude, obstacleMask))
            {
                agent.speed = 5; agent.destination = player.position; loseTimer = 2f; // нашёл, бежит 2 сек
            }
        }
        if (loseTimer > 0) { loseTimer -= Time.deltaTime; if (loseTimer <= 0) { agent.speed = 3.5f; Next(); } }
    }
}
