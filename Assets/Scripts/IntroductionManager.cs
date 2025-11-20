using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.InputSystem;

public class IntroductionManager : MonoBehaviour
{
    public State gameState;
    [HideInInspector]
    public enum State
    {
        start,
        calibration,
        wheelCheck,
        pedalCheck,
        blinkerCheck,
        restartCheck,
        end
    }
    public float StartTime = 2f;

    public LayerMask defaultCullingMask;

    [SerializeField] private PlayerInput input;
    [SerializeField] private LogitechInput carInput;
    [SerializeField] private CarMover car;
    public RectTransform introUI;

    [SerializeField] private UnityEvent onStart;
    [SerializeField] private UnityEvent onCali;
    [SerializeField] private UnityEvent onWheelCheck;
    [SerializeField] private UnityEvent onPedalCheck;
    [SerializeField] private UnityEvent onBlinkerCheck;
    [SerializeField] private UnityEvent onRestartCheck;
    [SerializeField] private UnityEvent onEnd;

    public AK.Wwise.Event tutorialChime;

    private bool rightWheelTurned = false;
    private bool leftWheelTurned = false;

    private bool gasPedalPressed = false;
    private bool brakePedalPressed = false;

    public bool rightBlinkerTurned = false;
    public bool leftBlinkerTurned = false;

    private bool calibrated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetState(State.start);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetState(State state)
    {
        gameState = state;

        switch (state)
        {
            case State.start:
                StartCoroutine(StartDelay());
                onStart.Invoke();
                break;

            case State.calibration:
                StartCoroutine(Calibration());
                onCali.Invoke();
                tutorialChime.Post(gameObject);
                break;

            case State.wheelCheck:
                onWheelCheck.Invoke();
                StartCoroutine(RightWheelCheck());
                StartCoroutine(LeftWheelCheck());
                tutorialChime.Post(gameObject);
                break;

            case State.pedalCheck:
                onPedalCheck.Invoke();
                StartCoroutine(SpeederCheck());
                StartCoroutine(BrakeCheck());
                tutorialChime.Post(gameObject);
                break;

            case State.blinkerCheck:
                onBlinkerCheck.Invoke();
                StartCoroutine(RightBlinkerCheck());
                StartCoroutine(LeftBlinkerCheck());
                tutorialChime.Post(gameObject);
                break;

            case State.restartCheck:
                StartCoroutine(RestartCheck());
                onRestartCheck.Invoke();
                tutorialChime.Post(gameObject);
                break;

            case State.end:
                onEnd.Invoke();
                tutorialChime.Post(gameObject);
                Invoke("GoToScene", 5f);
                break;
        }
    }

    void GoToScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    #region Start Delays
    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(StartTime);
        SetState(State.calibration);
    }
    #endregion

    private IEnumerator Calibration()
    {
        ForceXRRigLocation.OnCalibrationDone += () => calibrated = true;

        while (true)
        {
            if (ForceXRRigLocation.HasSavedCalibration && calibrated)
            {
                SetState(State.wheelCheck);
                yield break;
            }
            yield return null;
        }
    }

    #region Wheel Checks
    private IEnumerator RightWheelCheck()
    {
        while (true)
        {
            if (carInput.steeringWheelValue > 20000 || input.actions["Steer"].ReadValue<float>() > 0.1f)
            {
                CheckWheelTurned();
                rightWheelTurned = true;
            }
            yield return null;
        }
    }
    
    private IEnumerator LeftWheelCheck()
    {
        while (true)
        {
            if (carInput.steeringWheelValue < -20000 || input.actions["Steer"].ReadValue<float>() < -0.1f)
            {
                CheckWheelTurned();
                leftWheelTurned = true;
            }
            yield return null;
        }
    }

    void CheckWheelTurned()
    {
        if (rightWheelTurned && leftWheelTurned)
        {
            SetState(State.pedalCheck);
        }
    }
    #endregion

    #region Blinker Checks
    private IEnumerator RightBlinkerCheck()
    {
        while (true)
        {
            if (carInput.rightBlinker || input.actions["BlinkerRight"].ReadValue<float>() > 0.1f)
            {
                rightBlinkerTurned = true;
                CheckBlinkerTurned();
            }
            yield return null;
        }
    }

    private IEnumerator LeftBlinkerCheck()
    {
        while (true)
        {
            if (carInput.leftBlinker || input.actions["BlinkerLeft"].ReadValue<float>() > 0.1f)
            {
                leftBlinkerTurned = true;
                CheckBlinkerTurned();
            }
            yield return null;
        }
    }

    void CheckBlinkerTurned()
    {
        if (rightBlinkerTurned && leftBlinkerTurned)
        {
            SetState(State.restartCheck);
        }
    }
    #endregion

    #region Pedal Checks
    private IEnumerator SpeederCheck()
    {
        while (true)
        {
            if (carInput.speederValue < 0 || input.actions["GasPedal"].ReadValue<float>() > 0.1f)
            {
                gasPedalPressed = true;
                PedalCheck();
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator BrakeCheck()
    {
        while (true)
        {
            if (carInput.brakeValue < 0 || input.actions["BrakePedal"].ReadValue<float>() > 0.1f)
            {
                brakePedalPressed = true;
                PedalCheck();
                yield break;
            }
            yield return null;
        }
    }

    void PedalCheck()
    {
        if (gasPedalPressed && brakePedalPressed)
        {
            SetState(State.blinkerCheck);
        }
    }
    #endregion

    #region Helpers
    public void CameraCullingMask()
    {
        Camera.main.cullingMask = defaultCullingMask;
    }
    #endregion

    #region Restart Check

    IEnumerator RestartCheck()
    {
        while (gameState != State.end)
        {
            if (carInput.XboxButton)
            {
                SetState(State.end);
            }
            yield return null;
        }
    }

    #endregion

}
