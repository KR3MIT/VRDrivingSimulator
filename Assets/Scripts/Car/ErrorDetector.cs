using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using static TrafficLightManager;

public class ErrorDetector : MonoBehaviour
{
    public InstructionManager instructionManager;
    public TutorialTextScriptableObject TutorialText;
   public TrafficLightManager lightManager;
    public bool enteringCross = false;
    private Camera playerCamera;
    private bool backMirrorCheck = false;
    private  bool sideMirrorCheck = false;
    private bool shoulderCheck = false;
    private CarMover car;
    private DashboardController dashboard;

    [Header("Lane")]
    public GameObject waypointContainer;
    private List<Vector3> waypoints = new List<Vector3>();
    public bool laneDelay;
    public float laneThreshold = 2.5f;
    public float laneDelayHz = 2f;


    private void Start()
    {
        playerCamera = Camera.main;
        car = GetComponent<CarMover>();
        dashboard = GetComponent<DashboardController>();

        // Get all waypoints from the container
        foreach (Transform child in waypointContainer.transform)
        {
            waypoints.Add(child.position);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("StartLine") && (lightManager.currentState1 == TrafficLightState.Red || lightManager.currentState1 == TrafficLightState.Yellow))
        {
            FeedbackSystem.Instance.RegisterDrivingError("Kørte over for rødt lys.", "Du kørte over for rødt lys og det må man ikke.", DrivingError.ErrorSeverity.High);
        }
        else if(other.CompareTag("StartLine") && (lightManager.currentState1 == TrafficLightState.Green || lightManager.currentState1 == TrafficLightState.RedYellow))
        {
            enteringCross = true;
            Debug.Log("Entering crossroad");
        }
      
        if(other.CompareTag("StopLine"))
        {
           if(backMirrorCheck == false || sideMirrorCheck == false || shoulderCheck == false)
           {
                FeedbackSystem.Instance.RegisterDrivingError("Husk at orienter dig.", "Husk "+"spejl spejl skulder " +"for at orientere dig før du foretager et sving.", DrivingError.ErrorSeverity.Medium);
            }
           Debug.Log("Stop line crossed");
        }

        if (other.CompareTag("Pedestrian"))
        {
            Debug.Log("Pedestrian hit");
            instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorPedestrian);
            instructionManager.allowContinue = true;
            FeedbackSystem.Instance.RegisterDrivingError("Uagtsomt INGAME manddrab", "You killed someones child INGAME, someones loved one INGAME, someones parent INGAME.", DrivingError.ErrorSeverity.Extreme);
            //restart game?
        }
        else if (other.CompareTag("SplineCar"))
        {
            Debug.Log("Car hit");
            instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorCarCollision);
            instructionManager.allowContinue = true;
            FeedbackSystem.Instance.RegisterDrivingError("Bil kollision", "Du ramte en bil.", DrivingError.ErrorSeverity.Extreme);
            //restart game?
        }
      
    }
    public void CheckBlinker()
    {
       if(dashboard.CheckLeftBlinkerOn())
       {

            return;
       }
         else
         {
                FeedbackSystem.Instance.RegisterDrivingError("Glemte blinklys før du kørte ud i svinget.", "Husk at bruge dit blinklys før du foretager et sving.", DrivingError.ErrorSeverity.Medium);
         }
    }
    public void CheckOrientation()
    {
        while (!backMirrorCheck)
        {
            if (Physics.BoxCast(playerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), playerCamera.transform.forward, out RaycastHit hitInfo, playerCamera.transform.rotation, 100f))
            {
                if (hitInfo.collider.CompareTag("BackMirror"))
                {
                    backMirrorCheck = true;
                    Debug.Log("Back mirror checked");
                }
            }
        }
        while(!sideMirrorCheck)
        {
            if (Physics.BoxCast(playerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), playerCamera.transform.forward, out RaycastHit hitInfo, playerCamera.transform.rotation, 100f))
            {
                if (hitInfo.collider.CompareTag("SideMirror"))
                {
                    sideMirrorCheck = true;
                    Debug.Log("Side mirror checked");
                }
            }
        }
        while(!shoulderCheck)
        {
            if (Physics.BoxCast(playerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), playerCamera.transform.forward, out RaycastHit hitInfo, playerCamera.transform.rotation, 100f))
            {
                if (hitInfo.collider.CompareTag("Shoulder"))
                {
                    shoulderCheck = true;
                    Debug.Log("Shoulder checked");
                }
            }
        }
        //all checks done, set next check methods bool true
    }

    void Update()
    {

        //Debug.Log("Distance to: " + GetDistanceToPath(this.transform.position, waypoints));
        if(laneDelay==false)
        {
            if (GetDistanceToPath(this.transform.position, waypoints) > laneThreshold)
            {
                FeedbackSystem.Instance.RegisterDrivingError("Lane boundary violated", "Please stay within your lane.", DrivingError.ErrorSeverity.Medium);
                StartCoroutine(LaneError());
                Debug.Log("Lane Error registered");
            }
        }
        if (enteringCross)
        {
            enteringCross = false;
            CheckOrientation();
            CheckBlinker();
        }
        if (car.magnitude > 55f)
        {
            FeedbackSystem.Instance.RegisterDrivingError("Speeding.", "Watch your speed.", DrivingError.ErrorSeverity.Low);
            Debug.Log("Speeding Error registered");
        }
        

    }
    IEnumerator LaneError()
    {
        laneDelay = true;
        yield return new WaitForSeconds(laneDelayHz);
        laneDelay = false;
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
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }

        foreach (Vector3 waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint, 0.25f);
        }
    }
}





