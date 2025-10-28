using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class ErrorDetector : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pedestrian"))
        {
            Debug.Log("Collision with pedestrian detected!");
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collision with another car detected!");
        }
        if (other.CompareTag(""))
        {
            Debug.Log("Off-road driving detected!");
        }
    }
}

