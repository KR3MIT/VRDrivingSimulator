using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using static TrafficLightManager;
using static ObjectiveCard;

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
    private bool laneDelay;
    public float laneThreshold = 2.5f;
    private float laneDelayHz = 2f;
    private bool speedErrorDelay = false;
    private bool slowZoneDelay = false;
    private bool onlyStopOnce = false;
    public float slowSpeedLimit = 10f;
    private bool pavementDelay = false;
    private bool slowedDownForROW = false;
    public bool correctROW = true;


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
    void Update()
    {
        if (laneDelay==false)
        {
            if (GetDistanceToPath(this.transform.position, waypoints) > laneThreshold)
            {
                FeedbackSystem.Instance.FailAndLockObjectiveCard(ObjectiveType.LaneViolation);
                //FeedbackSystem.Instance.RegisterDrivingError("Vognbane overskridelse", "Du afveg fra din vognbane.", DrivingError.ErrorSeverity.Mellem);
                StartCoroutine(LaneError());
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
            FeedbackSystem.Instance.FailAndLockObjectiveCard(ObjectiveType.SpeedLimit);
            //FeedbackSystem.Instance.RegisterDrivingError("Fart overskridelse", "Fart for høj.", DrivingError.ErrorSeverity.Mellem);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (lightManager != null)
        {
            if (other.CompareTag("StartLine") && (lightManager.currentState1 == TrafficLightState.Red || lightManager.currentState1 == TrafficLightState.Yellow))
            {
                FeedbackSystem.Instance.FailAndLockObjectiveCard(ObjectiveType.TrafficLight);
                //FeedbackSystem.Instance.RegisterDrivingError("Kørte over for rødt.", "Husk at stoppe før stoplinjen hvis der er rødt lys og kør først ind i krydset ved grønt lys", DrivingError.ErrorSeverity.Høj);
            }
            else if (other.CompareTag("StartLine") && (lightManager.currentState1 == TrafficLightState.Green || lightManager.currentState1 == TrafficLightState.RedYellow))
            {
                enteringCross = true;
                //FeedbackSystem.Instance.RegisterDrivingError("Grønt lys", "Godt klaret! Du kørte først ind i krydset, når lyset var grønt.", DrivingError.ErrorSeverity.Korrekt);
            }

            if (other.CompareTag("StopLine"))
            {
                if (backMirrorCheck == false || sideMirrorCheck == false || shoulderCheck == false)
                {
                    if (onlyStopOnce == false)
                    {
                        onlyStopOnce = true;
                        FeedbackSystem.Instance.FailAndLockObjectiveCard(ObjectiveType.Orientation);
                        //FeedbackSystem.Instance.RegisterDrivingError("Husk at orientere dig.", "Husk spejl, spejl, skulder for at orientere dig før du foretager et sving.", DrivingError.ErrorSeverity.Mellem);
                    }
                }
                else
                {
                    if (onlyStopOnce == false)
                    {
                        //FeedbackSystem.Instance.RegisterDrivingError("Godt orienteret.", "Du huskede at bruge dine spejle og orientere dig før du foretog et sving.", DrivingError.ErrorSeverity.Korrekt);
                    }
                }
            }
            

           
        }
        if (other.CompareTag("Pedestrian"))
        {
            //if (instructionManager != null)
            //{
            //    instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorPedestrian);
            //    instructionManager.allowContinue = true;
            //}
            FeedbackSystem.Instance.FailAndLockObjectiveCard(ObjectiveType.PedestrianHit);
            //FeedbackSystem.Instance.RegisterDrivingError("Ramte en fodgænger", "Husk altid at orientere dig grundigt efter fodgængere.", DrivingError.ErrorSeverity.Ekstrem);
            GameEnder.Instance.EndGame(GameEnder.GameEndCondition.ExtremeError);
        }
        else if (other.CompareTag("SplineCar"))
        {
            //if(instructionManager != null)
            //{
            //    instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorCarCollision);
            //    instructionManager.allowContinue = true;
            //}
            FeedbackSystem.Instance.FailAndLockObjectiveCard(ObjectiveType.CarHit);
            //FeedbackSystem.Instance.RegisterDrivingError("Bil kollision", "Du ramte en bil, husk altid at orienter dig.", DrivingError.ErrorSeverity.Ekstrem);
            GameEnder.Instance.EndGame(GameEnder.GameEndCondition.ExtremeError);
        }

        else if (other.CompareTag("Pavement") && !pavementDelay)
        {
            StartCoroutine(PavementCollision());
        }
    }
   private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("SlowZone") && slowZoneDelay == false || other.CompareTag("Scenario2CarTrigger"))
        {
            if (car.magnitude * 3.6f <= slowSpeedLimit)
            {
                if (!slowedDownForROW)
                {
                    slowedDownForROW = true;
                }
                return;
            }
            slowZoneDelay = true;
            //FeedbackSystem.Instance.RegisterDrivingError("Vigepligt overtrædelse", "Husk at sænke hastigheden og orientere dig ved kryds med højre vigepligt.", DrivingError.ErrorSeverity.Mellem);
        }


    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SlowZone"))
        {
            if (!slowedDownForROW)
            {
                FeedbackSystem.Instance.FailAndLockObjectiveCard(ObjectiveType.RightOfWay);
            }
            slowedDownForROW = false;
            slowZoneDelay = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Pavement") && !pavementDelay)
        {
            Debug.Log("Pavement collision detected in OnCollisionEnter");
            StartCoroutine(PavementCollision());
        }
    }
    IEnumerator PavementCollision()
    {
        pavementDelay = true;
        Debug.Log("Pavement collision detected");
        FeedbackSystem.Instance.FailAndLockObjectiveCard(ObjectiveType.PavementHit);
        // FeedbackSystem.Instance.RegisterDrivingError("Kørte på fortovet.", "Husk at holde dig på vejen og undgå fortovet.", DrivingError.ErrorSeverity.Høj);
        yield return new WaitForSeconds(1f);
        pavementDelay = false;
    }
   
    
 

    public void CheckBlinker()
    {
       if(dashboard.CheckLeftBlinkerOn())
       {
            //FeedbackSystem.Instance.RegisterDrivingError("Korrekt brug af blinklys.", "Godt klaret! Du huskede at bruge dit blinklys før du foretog et sving.", DrivingError.ErrorSeverity.Korrekt);
            return;
       }
       else
       {
            FeedbackSystem.Instance.FailAndLockObjectiveCard(ObjectiveType.Blinkers);
            //FeedbackSystem.Instance.RegisterDrivingError("Glemte blinklys før du kørte ud i svinget.", "Husk at bruge dit blinklys før du foretager et sving.", DrivingError.ErrorSeverity.Mellem);
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
                }
            }
            yield return null;
        }
        //all checks done, set next check methods bool true
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





