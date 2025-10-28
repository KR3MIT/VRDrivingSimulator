using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class ErrorDetector : MonoBehaviour
{
    public InstructionManager instructionManager;
    public TutorialTextScriptableObject TutorialText;


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pedestrian"))
        {
            Debug.Log("Pedestrian hit");
            instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorPedestrian);
            instructionManager.allowContinue = true;
            //restart game?
        }
        else if (other.CompareTag("SplineCar"))
        {
            Debug.Log("Car hit");
            instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorCarCollision);
            instructionManager.allowContinue = true;
            //restart game?
        }
        else if (other.CompareTag("NOTURLANE"))
        {
            Debug.Log("Off road");
            instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorRoad);
            instructionManager.allowContinue = true;
            //restart game?
        }
    }
}





