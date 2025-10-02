using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// based on youtube tutorial by Game Dev Guide: https://www.youtube.com/watch?v=MXCZ-n5VyJc&t=647s

[InitializeOnLoad]
[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw default fields except branchProbability
        DrawPropertiesExcluding(serializedObject, "branchProbability");

        // Get branches property
        SerializedProperty branchesProp = serializedObject.FindProperty("branches");

        // Show branchProbability only if branches has at least one item
        if (branchesProp != null && branchesProp.arraySize > 0)
        {
            SerializedProperty branchProbProp = serializedObject.FindProperty("branchProbability");
            EditorGUILayout.PropertyField(branchProbProp);
        }

        serializedObject.ApplyModifiedProperties();
    }


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

        if (waypoint.branches != null)
        {
            // Draw blue lines to branch waypoints (from center)
            foreach (Waypoint branch in waypoint.branches)
            {
                if (branch != null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(waypoint.transform.position, branch.transform.position);
                }
            }
        }
    }
}
