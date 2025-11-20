using UnityEngine;

public class GameEnder : MonoBehaviour
{
    public event System.Action OnGameEnd;

    public CarMover car;
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
    public GameObject goodCard;
    public GameObject badCard;

    private bool hasPlacedGoodCard = false;
    private bool hasPlacedBadCard = false;

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
        //bool foundSpeedError = false;
        //bool foundROWError = false;

        //foreach (var error in FeedbackSystem.Instance.GetDrivingErrors())
        //{
        //    if (error.errorName == "Fart overskridelse")
        //    {
        //        foundSpeedError = true;
        //        continue;
        //    }
        //    if (error.errorName == "Vigepligt overtrædelse")
        //    {
        //        foundROWError = true;
        //        continue;
        //    }
        //}

        //if (!foundSpeedError)
        //{
        //    FeedbackSystem.Instance.RegisterDrivingError("Fartgrænse overholdt", "Du har overholdt fartgrænsen.", DrivingError.ErrorSeverity.Korrekt);
        //}
        //if( !foundROWError)
        //{
        //    FeedbackSystem.Instance.RegisterDrivingError("Vigepligt overholdt", "Du har overholdt din højrevigepligt.", DrivingError.ErrorSeverity.Korrekt);
        //}
        if (gameEnded) return;

        var data = FindFirstObjectByType<DataLog>();
        if (data != null)
            data.LogAllErrors();
        else
            Debug.LogWarning("data not logged | DataLog instance not found.");


        OnGameEnd?.Invoke();
        gameEnded = true;
        gameEndCanvas.enabled = true;

        foreach (var obj in FeedbackSystem.Instance.GetObjectiveLinks())
        {
            CreateObjectiveCard(obj);
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

        if(car != null)
            car.transmissionState = CarMover.TransmissionType.Park;
    }

    public void CreateObjectiveCard(ObjectiveLink obj)
    {
        if (!obj.isFailed && !hasPlacedGoodCard)
        {
            hasPlacedGoodCard = true;
            Instantiate(goodCard, errorContainer.transform);
        }
        else if (obj.isFailed && !hasPlacedBadCard)
        {
            hasPlacedBadCard = true;
            Instantiate(badCard, errorContainer.transform);
        }

            var errorObject = Instantiate(errorUIPrefab, errorContainer.transform);
        var errorCard = errorObject.GetComponent<ErrorCard>();

        errorCard.Initialize(obj.objectiveCard.titleText, obj.isFailed ? obj.objectiveCard.descriptionBadText : obj.objectiveCard.descriptionGoodText, obj.isFailed);
    }
}
