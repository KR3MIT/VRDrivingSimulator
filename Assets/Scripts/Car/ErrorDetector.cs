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
    public Camera playerCamera;
    private bool backMirrorCheck = false;
    private  bool sideMirrorCheck = false;
    private bool shoulderCheck = false;
    public CarMover car;
    public DashboardController dashboard;


    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("StartLine") && (lightManager.currentState1 == TrafficLightState.Red || lightManager.currentState1 == TrafficLightState.Yellow))
        {
            FeedbackSystem.Instance.RegisterDrivingError("Kørte over for rødt lys.", "Du kørte over for rødt lys og det må man ikke.", DrivingError.ErrorSeverity.High);
        }
        else if(other.CompareTag("StartLine") && (lightManager.currentState1 == TrafficLightState.Green || lightManager.currentState1 == TrafficLightState.RedYellow))
        {
            enteringCross = true;
        }
      
        if(other.CompareTag("StopLine"))
        {
           if(backMirrorCheck == false || sideMirrorCheck == false || shoulderCheck == false)
           {
                FeedbackSystem.Instance.RegisterDrivingError("Husk at orienter dig.", "Husk "+"spejl spejl skulder " +"for at orientere dig før du foretager et sving.", DrivingError.ErrorSeverity.Medium);
            }
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
                }
            }
        }
        //all checks done, set next check methods bool true
    }

    void Update()
    {
        if(enteringCross)
        {
            enteringCross = false;
            CheckOrientation();
            CheckBlinker();
        }
        if (car.magnitude > 55f)
            FeedbackSystem.Instance.RegisterDrivingError("Speeding.", "Watch your speed.", DrivingError.ErrorSeverity.Low);
        

    }
}





