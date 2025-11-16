using UnityEngine;
public class BuddyHotkeys : MonoBehaviour
{
    public Buddy buddy; public Transform player; public Transform distractMarker;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) buddy.SetFollow();
        if (Input.GetKeyDown(KeyCode.E)) buddy.SetWait();
        if (Input.GetKeyDown(KeyCode.R)) buddy.SetDistract(distractMarker);
    }
}