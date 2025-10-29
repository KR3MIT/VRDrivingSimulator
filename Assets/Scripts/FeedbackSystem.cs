using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public struct DrivingError
{
    public string errorName;
    public float timestamp;
    public string description;
    public ErrorSeverity severity;

    public enum ErrorSeverity
    {
        Extreme,
        High,
        Medium,
        Low,
    }

}

public class FeedbackSystem : MonoBehaviour
{
    
}
