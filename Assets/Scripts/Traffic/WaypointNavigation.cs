using UnityEngine;

public class WaypointNavigation : MonoBehaviour
{

    NPCController controller;
    public Waypoint currentWaypoint;
    [SerializeField]
    int direction = 1; // 1 for forward, -1 for backward

    private void Awake()
    {
        controller = GetComponent<NPCController>();
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //random value of either 0 or 1
        if (Random.value < 0.5f)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        controller.SetDestination(currentWaypoint.GetPosition());
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.reachedDestination)
        {
            bool shouldBranch = false;
            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
            {
                shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchProbability ? true : false;
            }
            if (shouldBranch)
            {
                int branchIndex = Random.Range(0, currentWaypoint.branches.Count);
                currentWaypoint = currentWaypoint.branches[branchIndex];
            }
            else
            {
                if (direction == 1)
                {
                    if (currentWaypoint.nextWaypoint != null)
                    {
                        currentWaypoint = currentWaypoint.nextWaypoint;
                        
                    }
                    else
                    {
                        direction = -1;
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    }
                }
               
                else if (direction == -1)
                {
                    if (currentWaypoint.previousWaypoint != null)
                    {
                        
                        currentWaypoint = currentWaypoint.previousWaypoint;
                    }
                    else
                    {
                        direction = 1;
                        currentWaypoint = currentWaypoint.nextWaypoint;
                    }
                }
            }
            controller.SetDestination(currentWaypoint.GetPosition());
        }
    }
}
