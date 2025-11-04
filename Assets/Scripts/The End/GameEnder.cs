using UnityEngine;

public class GameEnder : MonoBehaviour
{
    public static GameEnder Instance;
    private Canvas gameEndCanvas;
    public enum GameEndCondition
    {
        None,
        FinishZone,
        FailZone,
        ExtremeError,
        ManualEnd,
    }

    public GameEndCondition endCondition = GameEndCondition.None;
    public bool gameEnded = false;
    public GameObject errorUIPrefab;
    public GameObject errorContainer;

    

    private void Start()
    {
        gameEndCanvas = GetComponent<Canvas>();
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        //CreateErrorCard(new DrivingError("Sample Error", 0f, "This is a sample error description.", DrivingError.ErrorSeverity.Medium));
        //CreateErrorCard(new DrivingError("Sample Error", 0f, "This is a sample error description.", DrivingError.ErrorSeverity.Extreme));
        //CreateErrorCard(new DrivingError("Sample Error", 0f, "This is a sample error description.", DrivingError.ErrorSeverity.High));
    }
    public void EndGame(GameEndCondition condition)
    {
        FindFirstObjectByType<DataLog>().LogAllErrors();
        if (gameEnded) return;
        gameEnded = true;
        gameEndCanvas.enabled = true;
        foreach (var error in FeedbackSystem.Instance.GetDrivingErrors())
        {
            CreateErrorCard(error);
        }
        switch (condition)
        {
            case GameEndCondition.FinishZone:
                Debug.Log("Game Ended: Player reached the finish zone.");
                endCondition = GameEndCondition.FinishZone;
                break;
            case GameEndCondition.FailZone:
                Debug.Log("Game Ended: Player entered a fail zone.");
                endCondition = GameEndCondition.FailZone;
                break;
            case GameEndCondition.ExtremeError:
                Debug.Log("Game Ended: An extreme error was detected.");
                endCondition = GameEndCondition.ExtremeError;
                break;
            case GameEndCondition.ManualEnd:
                Debug.Log("Game Ended: Manually triggered.");
                endCondition = GameEndCondition.ManualEnd;
                break;
        }
        StartCoroutine(GameManager.Instance.ins.SmoothStop());
    }

    public void CreateErrorCard(DrivingError error)
    {
        var errorObject = Instantiate(errorUIPrefab, errorContainer.transform);
        var errorCard = errorObject.GetComponent<ErrorCard>();
        Color color;

        switch (error.severity)
        {
            case DrivingError.ErrorSeverity.Ekstrem:
                color = Color.red;
                break;
            case DrivingError.ErrorSeverity.Høj:
                color = new Color(1f, 0.5f, 0f); // Orange
                break;
            case DrivingError.ErrorSeverity.Mellem:
                color = Color.yellow;
                break;
            case DrivingError.ErrorSeverity.Lav:
                color = Color.white;
                break;
            case DrivingError.ErrorSeverity.Korrekt:
                color = Color.green;
                break;
            default:
                color = Color.gray;
            break;
        }

        errorCard.Initialize(error.errorName, error.description, error.severity.ToString(), color);
    }
}
