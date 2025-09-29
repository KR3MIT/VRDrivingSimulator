using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

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
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if (GUILayout.Button("Add Waypoint Before Selected"))
            {
                AddWaypointBeforeSelected();
            }
            if (GUILayout.Button("Add Waypoint After Selected"))
            {
                AddWaypointAfterSelected();
            }
            if (GUILayout.Button("Remove Selected Waypoint"))
            {
                RemoveSelectedWaypoint();
            }
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
            Waypoint prev = waypointRoot.GetChild(waypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint.previousWaypoint = prev;
            prev.nextWaypoint = waypoint;
            // Place the new waypoint in front of the previous one
            waypoint.transform.position = waypoint.previousWaypoint.transform.position;
            waypoint.transform.forward = waypoint.previousWaypoint.transform.forward;
            // Set width based on previous waypoint
            waypoint.width = prev.width;

        }
        else
        {
            waypoint.width = 1f; // Default width for the first waypoint
        }

            Selection.activeGameObject = waypoint.gameObject;
    }

    void AddWaypointBeforeSelected()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
        
        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;
        newWaypoint.width = selectedWaypoint.width;
        // Insert the new waypoint before the selected one in the hierarchy
        if (selectedWaypoint.previousWaypoint != null)
        {
            newWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
            selectedWaypoint.previousWaypoint.nextWaypoint = newWaypoint;
        }
        newWaypoint.nextWaypoint = selectedWaypoint;
        selectedWaypoint.previousWaypoint = newWaypoint;
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex());
        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void AddWaypointAfterSelected()
    {
        GameObject waypointObject = new GameObject("Waypoint " + waypointRoot.childCount, typeof(Waypoint));
        waypointObject.transform.SetParent(waypointRoot, false);
        Waypoint newWaypoint = waypointObject.GetComponent<Waypoint>();
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();

        waypointObject.transform.position = selectedWaypoint.transform.position;
        waypointObject.transform.forward = selectedWaypoint.transform.forward;
        newWaypoint.width = selectedWaypoint.width;
        // Insert the new waypoint after the selected one in the hierarchy
        if (selectedWaypoint.nextWaypoint != null)
        {
            newWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            selectedWaypoint.nextWaypoint.previousWaypoint = newWaypoint;
        }
        newWaypoint.previousWaypoint = selectedWaypoint;
        selectedWaypoint.nextWaypoint = newWaypoint;
        newWaypoint.transform.SetSiblingIndex(selectedWaypoint.transform.GetSiblingIndex() + 1);
        Selection.activeGameObject = newWaypoint.gameObject;
    }

    void RemoveSelectedWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
        if (selectedWaypoint.previousWaypoint != null)
        {
            selectedWaypoint.previousWaypoint.nextWaypoint = selectedWaypoint.nextWaypoint;
            Selection.activeGameObject = selectedWaypoint.previousWaypoint.gameObject;
        }
        if (selectedWaypoint.nextWaypoint != null)
        {
            selectedWaypoint.nextWaypoint.previousWaypoint = selectedWaypoint.previousWaypoint;
        }
        DestroyImmediate(selectedWaypoint.gameObject);
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


