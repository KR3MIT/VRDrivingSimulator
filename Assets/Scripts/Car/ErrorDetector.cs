using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class ErrorDetector : MonoBehaviour
{
    public InstructionManager instructionManager;
    public TutorialTextScriptableObject TutorialText;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pedestrian"))
        {
            instructionManager.ShowFreezeHint(0, true,TutorialText.ErrorPedestrian);
            instructionManager.allowContinue = true;
            Debug.Log("Collision with pedestrian detected!");
        }
        if (other.CompareTag("Player"))
        {
            instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorCarCollision);
            instructionManager.allowContinue = true;
            Debug.Log("Collision with another car detected!");
        }
        if (other.CompareTag("ErrorCollider"))
        {
            instructionManager.ShowFreezeHint(0, true, TutorialText.ErrorRoad);
            instructionManager.allowContinue = true;
            Debug.Log("Off-road driving detected!");
        }
    }
}

