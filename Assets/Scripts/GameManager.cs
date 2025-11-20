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
        onGameStart?.Invoke();
        StartCoroutine(SpeederCheck());
    }

    private IEnumerator SpeederCheck()
    {
        yield return new WaitForSeconds(2f);

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
        onGamePaused?.Invoke();
    }

    void EndState()
    {
        onGameEnd?.Invoke();
    }

    void RestartGame()
    {
        if (carInput.XboxButton)
        {
            if (gameState == State.playing)
            {
                Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if (gameState == State.end)
            {
                Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");
            }
        }
    }

    //public IEnumerator SmoothResume()
    //{

    //    Debug.Log("Resuming game");

    //    while (Time.timeScale < realTime && allowContinue)
    //    {
    //        Time.timeScale += Time.unscaledDeltaTime / resumeDuration * realTime;
    //        yield return null;
    //    }

    //    Time.timeScale = realTime;
    //    allowContinue = false;

    //}
    //public IEnumerator SmoothStop()
    //{
    //    Debug.Log("Freezing game");

    //    while (Time.timeScale > 0.1f && !allowContinue)
    //    {
    //        Time.timeScale -= Time.unscaledDeltaTime / resumeDuration * realTime;
    //        yield return null;
    //    }
    //    Time.timeScale = 0;
    //}

}
