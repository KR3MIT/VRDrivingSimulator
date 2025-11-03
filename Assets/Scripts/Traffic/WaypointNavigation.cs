using System.Collections;
using UnityEngine;

// based on youtube tutorial by Game Dev Guide: https://www.youtube.com/watch?v=MXCZ-n5VyJc&t=647s
public class WaypointNavigation : MonoBehaviour
{

    NPCController controller;
    public Waypoint currentWaypoint;
    public bool isCurrentlyCrossingRoad = false;
    [SerializeField]
    int direction = 1; // 1 for forward, -1 for backward

    public bool nextIsStop = false;
    public int nextIsExit = 0;

    private RaycastHit hit;
    private float crossingBoxcastDistance = 2f;
    private Vector3 crossingBoxcastScale = new Vector3(2f, 2f, 2f);
    private bool isCarBlockingCrossing = false;

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

        controller.SetDestination(currentWaypoint.GetPosition(direction, gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.reachedDestination)
        {

            if (isCurrentlyCrossingRoad) isCurrentlyCrossingRoad = false;

            if (currentWaypoint.trafficLightManager != null && nextIsStop == true)
            {
                CheckCrossing();
                return;
            }
            bool shouldBranch = false;
            int branchIndex = 0;
            if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
            {
                shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchProbability ? true : false;

                branchIndex = Random.Range(0, currentWaypoint.branches.Count);

                if (currentWaypoint.branches[branchIndex].trafficLightManager != null && currentWaypoint.trafficLightManager == null)
                {
                    if (!currentWaypoint.branches[branchIndex].CanEnterWaypoint())
                    {
                        shouldBranch = false;
                    }
                    //else if(nextIsExit > 0)
                    //{
                    //    Debug.Log("nuh uh");
                    //}
                    //else
                    //{
                    //    currentWaypoint.branches[branchIndex].EnterWaypoint(gameObject);
                    //    Debug.Log("Entered waypoint ");
                    //}
                }

                if (nextIsExit > 0)
                {
                    nextIsExit--;
                    if(nextIsExit == 0) 
                    { 
                        shouldBranch = false; 
                    }
                }
            }
            if (shouldBranch)
            {
                if (currentWaypoint.branches[branchIndex].trafficLightManager != null && currentWaypoint.trafficLightManager == null)
                {
                    nextIsStop = true;
                    currentWaypoint.branches[branchIndex].EnterWaypoint(gameObject);
                    //Debug.Log("Entered waypoint ");
                }

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
            controller.SetDestination(currentWaypoint.GetPosition(direction, gameObject));
        }
    }

        void CheckCrossing() 
    {
        // Perform a box cast forward from the car's position to detect other cars
        if (Physics.BoxCast(transform.position, crossingBoxcastScale, transform.forward, out hit, transform.rotation, crossingBoxcastDistance))
        {
            if (hit.collider.CompareTag("SplineCar"))
            {
                //Debug.Log("Car blocking crossing");
                isCarBlockingCrossing = true;
            }
            else
            {
                isCarBlockingCrossing = false;
            }
        }

        if (currentWaypoint.trafficLightIndex == 1 && !isCarBlockingCrossing)
        {
            if(currentWaypoint.trafficLightManager.CheckLightState1() == TrafficLightManager.TrafficLightState.Green)
            {
                DoThings();
            }
            // else wait
        }
        else if (currentWaypoint.trafficLightIndex == 2 && !isCarBlockingCrossing)
        {
            if (currentWaypoint.trafficLightManager.CheckLightState2() == TrafficLightManager.TrafficLightState.Green)
            {
                DoThings();
            }
            // else wait
        }

        var nextWaypoint = currentWaypoint.nextWaypoint != null ? currentWaypoint.nextWaypoint : currentWaypoint.previousWaypoint;
            controller.RotateTowards(nextWaypoint.transform.position);

        void DoThings()
        {
            //Debug.Log("Crossing the road");
            isCurrentlyCrossingRoad = true;
            currentWaypoint.ExitWaypoint(gameObject);

            currentWaypoint = currentWaypoint.nextWaypoint != null ? currentWaypoint.nextWaypoint : currentWaypoint.previousWaypoint;
            
            controller.SetDestination(currentWaypoint.GetPosition(direction, gameObject));
            direction = Random.Range(0, 2) == 0 ? 1 : -1;

            nextIsStop = false;
            nextIsExit = 2;

        }
    }
}
