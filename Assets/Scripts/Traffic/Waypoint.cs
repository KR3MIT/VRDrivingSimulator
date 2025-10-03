using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// based on youtube tutorial by Game Dev Guide: https://www.youtube.com/watch?v=MXCZ-n5VyJc&t=647s

public class Waypoint : MonoBehaviour
{
   public Waypoint previousWaypoint;
   public Waypoint nextWaypoint;

    [Range (0f, 5f)]
    public float width = 1f;

    public List<Waypoint> branches = new List<Waypoint>();

    [SerializeField]
    private List<GameObject> pedestriansOnWaypoint = new List<GameObject>();
    public int maxPedestrians = 3;

    [Range(0f, 1f)]
    public float branchProbability = 0.5f;

    [Header("Traffic Light Settings")]
    public TrafficLightManager trafficLightManager;
    [Range(1,2)]
    public int trafficLightIndex = 1;
    
    public bool CanEnterWaypoint()
    {
        return pedestriansOnWaypoint.Count < maxPedestrians;
    }

    public void EnterWaypoint(GameObject pedestrian)
    {
        if (!pedestriansOnWaypoint.Contains(pedestrian) && CanEnterWaypoint())
        {
            pedestriansOnWaypoint.Add(pedestrian);
        }
    }
    public void ExitWaypoint(GameObject pedestrian)
    {
        if (pedestriansOnWaypoint.Contains(pedestrian))
        {
            pedestriansOnWaypoint.Remove(pedestrian);
        }
    }

    public Vector3 GetWaitSpot(int index)
    {
        Vector3 right = transform.right * (width / 2f);
        Vector3 leftPoint = transform.position - right;
        Vector3 rightPoint = transform.position + right;
        if (index <= 0) index = 1;
        if (index > maxPedestrians) index = maxPedestrians;
        return Vector3.Lerp(leftPoint, rightPoint, (float)index / (maxPedestrians + 1));
    }

    public Vector3 GetPosition(int direction)
    {
        Vector3 maxBound;
        Vector3 minBound;
        if (direction > 0)
        {
            minBound = transform.position + transform.right * width / 5f;
            maxBound = transform.position + transform.right * width / 2f;
        }
        else
        {
            minBound = transform.position - transform.right * width / 2f;
            maxBound = transform.position - transform.right * width / 5f;
        }


            return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
    }


}
