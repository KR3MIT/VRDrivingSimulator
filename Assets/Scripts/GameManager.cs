using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public State gameState;
    public enum State
    {
        start,
        playing,
        paused,
        end
    }

    public InstructionManager ins;
    [SerializeField] private LogitechInput carInput;
    [SerializeField] private PlayerInput input;
    [SerializeField] private CarMover car;

    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onGamePlaying;
    [SerializeField] private UnityEvent onGamePaused;
    [SerializeField] private UnityEvent onGameEnd;

    private float realTime = 1f;
    private float resumeDuration = 2f;
    private bool allowContinue = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }


        if (ins == null)
        {
            Debug.LogError("InstructionManager reference is missing in GameManager.");
            return;
        }
        else if (carInput == null)
        {
            Debug.LogError("LogitechInput reference is missing in GameManager.");
            return;
        }

        SetState(State.start);
    }

    // Update is called once per frame
    void Update()
    {
        RestartGame();
    }

    public void SetState(State state)
    {
        gameState = state;

        switch (state)
        {
            case State.start:
                StartState();
                break;
            case State.playing:
                PlayState();
                break;
            case State.paused:
                PausedState();
                break;
            case State.end:
                EndState();
                break;
        }
    }

    void StartState()
    {
        car.transmissionState = CarMover.TransmissionType.Park;
        Debug.Log("Game " + car.transmissionState);
        onGameStart?.Invoke();
        StartCoroutine(SpeederCheck());
        ins.StartCoroutine(ins.SmoothResume());
    }

    private IEnumerator SpeederCheck()
    {
        while (true)
        {
            if (carInput.speederValue < 0 || input.actions["GasPedal"].ReadValue<float>() > 0.1f)
            {
                SetState(State.playing);
                yield break;
            }
            yield return null;
        }
    }

    void PlayState()
    {
        car.transmissionState = CarMover.TransmissionType.Drive;
        Debug.Log("Game2 " + car.transmissionState);
        onGamePlaying?.Invoke();
    }

    void PausedState()
    {
        ins.StartCoroutine(ins.SmoothStop());
        onGamePaused?.Invoke();
    }

    void EndState()
    {
        ins.StartCoroutine(ins.SmoothStop());
        onGameEnd?.Invoke();
    }

    void RestartGame()
    {
        if (gameState != State.playing)
            return;

        if (carInput.StartButton && carInput.RSB)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    public IEnumerator SmoothResume()
    {

        Debug.Log("Resuming game");

        while (Time.timeScale < realTime && allowContinue)
        {
            Time.timeScale += Time.unscaledDeltaTime / resumeDuration * realTime;
            yield return null;
        }

        Time.timeScale = realTime;
        allowContinue = false;

    }
    public IEnumerator SmoothStop()
    {

        Debug.Log("Freezing game");

        while (Time.timeScale > 0.1f && !allowContinue)
        {
            Time.timeScale -= Time.unscaledDeltaTime / resumeDuration * realTime;
            yield return null;
        }

        Time.timeScale = 0;
       // allowContinue = true;
    }

}
