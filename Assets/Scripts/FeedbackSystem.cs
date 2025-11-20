using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ObjectiveCard;

public struct ObjectiveLink
{
    public ObjectiveCard objectiveCard;
    public bool isFailed;
    public bool isLocked;

    public ObjectiveLink(ObjectiveCard card)
    {
        objectiveCard = card;
        isFailed = false;
        isLocked = false;
    }
}

public class FeedbackSystem : MonoBehaviour
{
    public static FeedbackSystem Instance;
    //private HashSet<DrivingError> drivingErrors = new HashSet<DrivingError>();
    [SerializeField]private List<ObjectiveCard> objectiveCards = new List<ObjectiveCard>();
    private List<ObjectiveLink> objectiveLinks = new List<ObjectiveLink>();

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

        foreach (var card in objectiveCards)
        {
            objectiveLinks.Add(new ObjectiveLink(card));
        }
    }

    public void FailAndLockObjectiveCard(ObjectiveType type)
    {
        for (int i = 0; i < objectiveLinks.Count; i++)
        {
            if (objectiveLinks[i].objectiveCard.objectiveType == type && !objectiveLinks[i].isLocked)
            {
                var link = objectiveLinks[i];
                link.isFailed = true;
                link.isLocked = true;
                objectiveLinks[i] = link;
            }
        }
    }

    public List<ObjectiveLink> GetObjectiveLinks()
    {
        return objectiveLinks;
    }

    //public void RegisterDrivingError(string name = "", string desc = "", DrivingError.ErrorSeverity sev = DrivingError.ErrorSeverity.Ingen)
    //{
    //    drivingErrors.Add(new DrivingError(name, 0f, desc, sev));
    //    Debug.Log("Registered Driving Error: " + name + " - " + desc + " - Severity: " + sev.ToString());
    //}

    //public HashSet<DrivingError> GetDrivingErrors()
    //{
    //    return drivingErrors;
    //}

}
