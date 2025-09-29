using UnityEditor;
using UnityEngine;

public class WaypointManagerWindow : EditorWindow
{
    [MenuItem("Window/Waypoint Manager")]
    public static void OpenWindow()
    {
        GetWindow<WaypointManagerWindow>();
    }

    public Transform waypointRoot;

    private void OnGUI()
    {
        GUILayout.Label("Waypoint Manager", EditorStyles.boldLabel);
        waypointRoot = (Transform)EditorGUILayout.ObjectField("Waypoint Root", waypointRoot, typeof(Transform), true);

        if (waypointRoot == null)
        {
            EditorGUILayout.HelpBox("Please assign a Waypoint Root Transform.", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }
    }
        void DrawButtons()
    {
        if (GUILayout.Button("Add Waypoint"))
        {
            AddWaypoint();
        }
        /*if (GUILayout.Button("Clear Waypoints"))
          {
              ClearWaypoints();
          }
          */
    }
    void AddWaypoint()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);
        Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
        if (waypointRoot.childCount > 1)
        {
            waypoint.previousWaypoint = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.previousWaypoint.nextWaypoint = waypoint;
            // Place the new waypoint 2 units in front of the previous one
            waypoint.transform.position = waypoint.previousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.previousWaypoint.transform.forward;
            
        }
      
        Selection.activeGameObject = waypoint.gameObject;
    }

   /* void ClearWaypoints()
    {
        if (EditorUtility.DisplayDialog("Clear Waypoints", "Are you sure you want to delete all waypoints?", "Yes", "No"))
        {
            while (waypointRoot.childCount > 0)
            {
                DestroyImmediate(waypointRoot.GetChild(0).gameObject);
            }
        }
    }
   */

    }


