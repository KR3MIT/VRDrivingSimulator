using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public State gameState;
    public enum State
    {
        start,
        playing,
        paused,
        end
    }

    [SerializeField] private InstructionManager ins;
    [SerializeField] private LogitechInput input;
    [SerializeField] private CarMover car;

    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onGamePlaying;
    [SerializeField] private UnityEvent onGamePaused;
    [SerializeField] private UnityEvent onGameEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(ins == null)
        {
            Debug.LogError("InstructionManager reference is missing in GameManager.");
            return;
        }
        else if (input == null)
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
            if (input.speederValue < 0)
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
        if(gameState != State.playing)
            return;

        if (input.StartButton && input.RSB)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
