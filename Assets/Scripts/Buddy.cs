using UnityEngine;
using UnityEngine.AI;

public class Buddy : MonoBehaviour
{
    public Transform player;
    public Transform distractPoint;
    public enum Order { Follow, Wait, Distract }
    public Order current;
    NavMeshAgent agent;

    void Awake() { agent = GetComponent<NavMeshAgent>(); }
    void Update()
    {
        switch (current)
        {
            case Order.Follow: agent.destination = player.position; break;
            case Order.Wait: agent.ResetPath(); break;
            case Order.Distract: agent.destination = distractPoint.position; break;
        }
    }
    public void SetFollow() => current = Order.Follow;
    public void SetWait() => current = Order.Wait;
    public void SetDistract(Transform t) { distractPoint = t; current = Order.Distract; }
}