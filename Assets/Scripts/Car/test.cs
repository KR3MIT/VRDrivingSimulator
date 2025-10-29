using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    public GameObject waypointContainer;

    private List<Vector3> waypoints = new List<Vector3>();

    public void Start()
    {
        foreach (Transform child in waypointContainer.transform)
        {
            waypoints.Add(child.gameObject.transform.position);
        }
    }


    public void Update()
    {
        Debug.Log("Distance to: " + GetDistanceToPath(this.transform.position, waypoints));
    }

    public float GetDistanceToPath(Vector3 carPosition, List<Vector3> waypoints)
    {
        float minDistance = float.MaxValue;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Vector3 start = waypoints[i];
            Vector3 end = waypoints[i + 1];
            Vector3 lineDirection = (end - start).normalized;
            float lineLength = Vector3.Distance(start, end);
            Vector3 toCar = carPosition - start;
            float projectionLength = Vector3.Dot(toCar, lineDirection);
            projectionLength = Mathf.Clamp(projectionLength, 0, lineLength);
            Vector3 closestPoint = start + lineDirection * projectionLength;
            float distance = Vector3.Distance(carPosition, closestPoint);
            if (distance < minDistance)
                minDistance = distance;
        }
        return minDistance;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        for(int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }

        foreach (Vector3 waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint, 0.25f);
        }
    }
}
