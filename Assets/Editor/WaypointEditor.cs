using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class WaypointEditor
{
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmos(Waypoint waypoint, GizmoType gizmoType)
    {
        // Set color for the sphere
        Gizmos.color = ((gizmoType & GizmoType.Selected) != 0) ? Color.yellow : Color.yellow * 0.5f;
        Gizmos.DrawSphere(waypoint.transform.position, 0.1f);

        // Calculate left and right points of the width line
        Vector3 right = waypoint.transform.right * (waypoint.width / 2f);
        Vector3 leftPoint = waypoint.transform.position - right;
        Vector3 rightPoint = waypoint.transform.position + right;

        // Draw the width line (white)
        Gizmos.color = Color.white;
        Gizmos.DrawLine(leftPoint, rightPoint);

        // Draw red line to previous waypoint (left side)
        if (waypoint.previousWaypoint != null)
        {
            Vector3 prevLeft = waypoint.previousWaypoint.transform.position - waypoint.previousWaypoint.transform.right * (waypoint.previousWaypoint.width / 2f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(leftPoint, prevLeft);
        }

        // Draw green line to next waypoint (right side)
        if (waypoint.nextWaypoint != null)
        {
            Vector3 nextRight = waypoint.nextWaypoint.transform.position + waypoint.nextWaypoint.transform.right * (waypoint.nextWaypoint.width / 2f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(rightPoint, nextRight);
        }
    }
}
