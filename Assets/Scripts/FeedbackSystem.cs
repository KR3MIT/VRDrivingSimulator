using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FeedbackSystem : MonoBehaviour
{

    public string errorLogName = "errorLog.txt";

    public TMP_Text feedbackText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string filepath = Path.Combine(Application.persistentDataPath, errorLogName);
        Debug.Log(Application.persistentDataPath);

        if (File.Exists(filepath))
        {
            string feedback = File.ReadAllText(filepath);
            feedbackText.text = feedback;
        } else
        {
            feedbackText.text = "No error log found.";
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
