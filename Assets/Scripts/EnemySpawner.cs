using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int waveCount = 5;          // сколько врагов заспавнить при волне
    public float spreadRadius = 2f;    // разброс вокруг поинта

    public void SpawnWave()
    {
        if (!enemyPrefab || spawnPoints.Length == 0) return;

        for (int i = 0; i < waveCount; i++)
        {
            Transform basePoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 pos = basePoint.position + Random.insideUnitSphere * spreadRadius;
            pos.y = basePoint.position.y;

            var go = Instantiate(enemyPrefab, pos, basePoint.rotation);

            // если используешь NavMeshAgent, можно сразу поправить позицию
            var agent = go.GetComponent<NavMeshAgent>();
            if (agent && agent.isOnNavMesh == false)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(pos, out hit, 3f, NavMesh.AllAreas))
                    go.transform.position = hit.position;
            }
        }
    }

    // дополнительный спавн р€дом с точкой (дл€ WatcherТа)
    public void SpawnFewAt(Vector3 around, int count = 2)
    {
        if (!enemyPrefab) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = around + Random.insideUnitSphere * spreadRadius;
            var go = Instantiate(enemyPrefab, pos, Quaternion.identity);

            var agent = go.GetComponent<NavMeshAgent>();
            if (agent && agent.isOnNavMesh == false)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(pos, out hit, 3f, NavMesh.AllAreas))
                    go.transform.position = hit.position;
            }
        }
    }
}