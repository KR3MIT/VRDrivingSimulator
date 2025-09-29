using UnityEngine;

public class WaypointNavigation : MonoBehaviour
{

    NPCController controller;
    public Waypoint currentWaypoint;

    private void Awake()
    {
        controller = GetComponent<NPCController>();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller.SetDestination(currentWaypoint.GetPosition());
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.reachedDestination)
        {
            currentWaypoint = currentWaypoint.nextWaypoint;
            controller.SetDestination(currentWaypoint.GetPosition());
        }
    }
}
