using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataLog : MonoBehaviour
{
    private const string LogsDir = "Assets/DataLogs";
    private StringBuilder _sb;
    private string _fileName;
    private string _filePath;

    public string playerName = "NAME_HERE";
    private int attemptNumber = 1;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

        EnsureInitialized();
        
    }

    private void IncrementAttemptForCurrentScene()
    {
        // Use a stable PlayerPrefs key per player and scene. Sanitize playerName for key safety.
        string sceneName = SceneManager.GetActiveScene().name;
        string safePlayer = string.IsNullOrEmpty(playerName) ? "PLAYER" : playerName.Replace(" ", "_");
        string key = $"AttemptCount_{safePlayer}_{sceneName}";

        int stored = PlayerPrefs.GetInt(key, 0);
        stored++; // this Start() indicates a new attempt for this scene
        PlayerPrefs.SetInt(key, stored);
        PlayerPrefs.Save();

        attemptNumber = stored;
    }



    // Ensure header and file are created without calling LogError (prevents recursion)
    public void EnsureInitialized()
    {
        if (_sb != null) return;

        if (!Directory.Exists(LogsDir))
        {
            Directory.CreateDirectory(LogsDir);
        }
        // load player name saved by MainMenuUI (if any)
        playerName = PlayerPrefs.GetString("PlayerName", playerName);
        IncrementAttemptForCurrentScene();

        _fileName = "DrivingErrorLog_" + playerName + "_" + SceneManager.GetActiveScene().name + "_Attempt " + attemptNumber + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        _filePath = Path.Combine(LogsDir, _fileName + ".txt");

        _sb = new StringBuilder();
        _sb.AppendLine("Scenario: " + SceneManager.GetActiveScene().name + " - Driving Errors:");
        _sb.AppendLine();
        _sb.AppendLine("User: " + playerName);
        _sb.AppendLine("Attempt: " + attemptNumber);
        _sb.AppendLine();
        _sb.AppendLine("Time Taken: " + Time.timeSinceLevelLoad);
        _sb.AppendLine();

        // Use column headings that match the requested layout
        // Columns: ERROR_NAME | ERROR_TIME | SEVERITY_LEVEL | ERROR_DESCRIPTION
        _sb.AppendLine("ERROR_NAME".PadRight(30) + "ERROR_TIME".PadRight(15) + "SEVERITY_LEVEL".PadRight(25) + "ERROR_DESCRIPTION");
        _sb.AppendLine();

        File.WriteAllText(_filePath, _sb.ToString());
    }

    // Call this method when an error occurs. It appends the error entry (name, time, severity, description) on a single aligned row.
    public void LogAllErrors()
    {
        EnsureInitialized();

        if (FeedbackSystem.Instance == null)
        {
            Debug.LogWarning("DataLog.LogAllErrors: FeedbackSystem.Instance is null - no errors to log.");
            File.WriteAllText(_filePath, _sb.ToString());
            return;
        }

        var errors = FeedbackSystem.Instance.GetDrivingErrors();
        foreach (var error in errors)
        {
            string nameCol = (error.errorName ?? string.Empty).PadRight(30);
            string timeCol = error.timestamp.ToString("0.##").PadRight(15);
            string sevText = error.severity.ToString().ToUpperInvariant();
            string sevCol = $"[{sevText}]".PadRight(25);
            string descCol = error.description ?? string.Empty;

            _sb.AppendLine($"{nameCol}{timeCol}{sevCol}{descCol}");
            _sb.AppendLine(); // blank line after each entry to match previous layout
        }

        File.WriteAllText(_filePath, _sb.ToString());
    }

    // Keep existing helper if other code uses it
    public void CreateTextFile(string fileName, string content)
    {
        string path = Path.Combine(LogsDir, fileName + ".txt");
        File.WriteAllText(path, content);
    }
}
