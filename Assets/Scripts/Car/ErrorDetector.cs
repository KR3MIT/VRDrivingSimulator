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

    public LayerMask mirrorLayer;

    [Header("Lane")]
    public GameObject waypointContainer;
    private List<Vector3> waypoints = new List<Vector3>();
    public bool laneDelay;
    public float laneThreshold = 2.5f;
    public float laneDelayHz = 2f;
    public bool speedErrorDelay = false;

    private bool onlyStopOnce = false;


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
            Debug.Log("Ran a red");
            FeedbackSystem.Instance.RegisterDrivingError("Kørte over for rødt.", "Husk først at køre ind i et kryds, når lyset bliver grønt", DrivingError.ErrorSeverity.Høj);
        }
        else if(other.CompareTag("StartLine") && (lightManager.currentState1 == TrafficLightState.Green || lightManager.currentState1 == TrafficLightState.RedYellow))
        {
            enteringCross = true;
            Debug.Log("Entering crossroad");
            FeedbackSystem.Instance.RegisterDrivingError("Grønt lys", "Godt klaret! Du kørte først ind i krydset, når lyset var grønt.", DrivingError.ErrorSeverity.Korrekt);
        }
      
        if(other.CompareTag("StopLine"))
        {
           if(backMirrorCheck == false || sideMirrorCheck == false || shoulderCheck == false)
           {
                if(onlyStopOnce==false)
                {
                    onlyStopOnce = true;
                    FeedbackSystem.Instance.RegisterDrivingError("Husk at bruge spejle.", "Husk spejl spejl skulder for at orientere dig før du foretager et sving.", DrivingError.ErrorSeverity.Mellem);
                }
                
           }
           else
           {
                if (onlyStopOnce == false)
                {
                    FeedbackSystem.Instance.RegisterDrivingError("Godt orienteret.", "Du huskede at bruge dine spejle og orientere dig før du foretog et sving.", DrivingError.ErrorSeverity.Korrekt);
                }
           }

           Debug.Log("Stop line crossed");
        }

        if (other.CompareTag("Pedestrian"))
        {
            Debug.Log("Pedestrian hit");
            instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorPedestrian);
            instructionManager.allowContinue = true;
            FeedbackSystem.Instance.RegisterDrivingError("Uagtsomt manddrab", "Husk altid at orientere dig grundigt efter fodgængere.", DrivingError.ErrorSeverity.Ekstrem);
            //restart game?
        }
        else if (other.CompareTag("SplineCar"))
        {
            Debug.Log("Car hit");
            instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorCarCollision);
            instructionManager.allowContinue = true;
            FeedbackSystem.Instance.RegisterDrivingError("Bil kollision", "Du ramte en bil.", DrivingError.ErrorSeverity.Ekstrem);
            //restart game?
        }
      
    }
    public void CheckBlinker()
    {
       if(dashboard.CheckLeftBlinkerOn())
       {
            Debug.Log("venstre blinker tændt ved check");
            FeedbackSystem.Instance.RegisterDrivingError("Korrekt brug af blinklys.", "Godt klaret! Du huskede at bruge dit blinklys før du foretog et sving.", DrivingError.ErrorSeverity.Korrekt);
            return;
       }
       else
       {
            FeedbackSystem.Instance.RegisterDrivingError("Glemte blinklys før du kørte ud i svinget.", "Husk at bruge dit blinklys før du foretager et sving.", DrivingError.ErrorSeverity.Mellem);
       }
    }
    public IEnumerator CheckOrientation()
    {
        //Debug.Log("Starting mirror checks");
        backMirrorCheck = true;
        //while (!backMirrorCheck)
        //{
        //    if (Physics.BoxCast(playerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), playerCamera.transform.forward, out RaycastHit hitInfo, playerCamera.transform.rotation, 100f, mirrorLayer))
        //    {
        //        Debug.Log("Boxcast hit: " + hitInfo.collider.name);
        //        if (hitInfo.collider.CompareTag("BackMirror"))
        //        {
        //            backMirrorCheck = true;
        //            Debug.Log("Back mirror checked");
        //        }
        //    }
        //    yield return new WaitForSeconds(.2f);
        //}
        while (!sideMirrorCheck)
        {
            if (Physics.BoxCast(playerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), playerCamera.transform.forward, out RaycastHit hitInfo, playerCamera.transform.rotation, 100f, mirrorLayer))
            {
                if (hitInfo.collider.CompareTag("SideMirror"))
                {
                    sideMirrorCheck = true;
                    Debug.Log("Side mirror checked");
                }
            }
            yield return null;
        }
        while(!shoulderCheck)
        {
            if (Physics.BoxCast(playerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), playerCamera.transform.forward, out RaycastHit hitInfo, playerCamera.transform.rotation, 100f, mirrorLayer))
            {
                if (hitInfo.collider.CompareTag("Shoulder"))
                {
                    shoulderCheck = true;
                    Debug.Log("Shoulder checked");
                }
            }
            yield return null;
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
                FeedbackSystem.Instance.RegisterDrivingError("Lane boundary violated", "Please stay within your lane.", DrivingError.ErrorSeverity.Mellem);
                StartCoroutine(LaneError());
                Debug.Log("Lane Error registered");
            }
        }
        if (enteringCross)
        {
            enteringCross = false;
            StartCoroutine(CheckOrientation());
            CheckBlinker();
        }
        if (car.magnitude *3.6f > 55f && speedErrorDelay == false)
        {
            speedErrorDelay = true;
            FeedbackSystem.Instance.RegisterDrivingError("Speed warning", "Too much speed.", DrivingError.ErrorSeverity.Mellem);
            Debug.Log("Speed Error registered");
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





