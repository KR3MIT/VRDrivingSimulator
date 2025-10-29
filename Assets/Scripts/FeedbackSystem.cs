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
        None,
    }

    public DrivingError(string name = "", float time = 0f, string desc = "", ErrorSeverity sev = ErrorSeverity.None)
    {
        errorName = name;
        timestamp = time;
        description = desc;
        severity = sev;
    }

}

public class FeedbackSystem : MonoBehaviour
{
    
}
