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

    public TrafficLightManager trafficLightManager;
    [Range(1,2)]
    public int trafficLightIndex = 1;
    

    [Range (0f, 1f)]
    public float branchProbability = 0.5f;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * width / 2f;
        Vector3 maxBound = transform.position - transform.right * width / 2f;

        return Vector3.Lerp(minBound,maxBound,Random.Range(0f,1f));
    }


}
