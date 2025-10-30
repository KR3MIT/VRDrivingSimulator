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
    public static FeedbackSystem Instance;
    private List<DrivingError> drivingErrors = new List<DrivingError>();

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterDrivingError(string name = "", string desc = "", DrivingError.ErrorSeverity sev = DrivingError.ErrorSeverity.None)
    {
        drivingErrors.Add(new DrivingError(name, 0f, desc, sev));
    }

    public List<DrivingError> GetDrivingErrors()
    {
        return drivingErrors;
    }

}
