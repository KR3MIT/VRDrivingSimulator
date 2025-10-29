using NUnit.Framework;
using System.Collections.Generic;
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
    public FeedbackSystem instance;
    private List<DrivingError> drivingErrors = new List<DrivingError>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterDrivingError(string name = "", float time = 0f, string desc = "", DrivingError.ErrorSeverity sev = DrivingError.ErrorSeverity.None)
    {
        drivingErrors.Add(new DrivingError(name, time, desc, sev));
    }
}
