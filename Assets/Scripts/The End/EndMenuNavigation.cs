using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EndMenuNavigation : MonoBehaviour
{
    public LogitechInput logitechInput;
    public PlayerInput input;
    private float lastInputTime = 0f;
    private float inputDelay = 0.1f; // Delay between input reads
    private int objectiveCount;
    public Scrollbar scrollbar;
    private float scrollbarSensitivity = 0f;
    private GameEnder gameEnder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objectiveCount = FeedbackSystem.Instance.GetObjectiveLinks().Count + 1 + 1;
        scrollbar.value = 1f;
        CalculateScrollbarSensitivity();

        if (logitechInput == null)
            Debug.LogWarning("LogitechInput reference is missing in EndMenuNavigation.");
        
        if (input == null)
            Debug.LogWarning("PlayerInput reference is missing in EndMenuNavigation.");

        gameEnder = GetComponent<GameEnder>();
        gameEnder.OnGameEnd += OnEnd;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastInputTime > inputDelay)
        {
            if (logitechInput.dpadValue == 1 || input.actions["GasPedal"].ReadValue<float>() > 0.1f) // Up
            {
                lastInputTime = Time.time;
                if (scrollbar.value + scrollbarSensitivity > 1)
                {
                    scrollbar.value = 1f;
                }
                else
                {
                    scrollbar.value += scrollbarSensitivity;
                }
            }
            else if (logitechInput.dpadValue == -1 || input.actions["BrakePedal"].ReadValue<float>() > 0.1f) // Down
            {
                lastInputTime = Time.time;

                if (scrollbar.value - scrollbarSensitivity < 0)
                {
                    scrollbar.value = 0f;
                }
                else
                {
                    scrollbar.value -= scrollbarSensitivity;
                }
            }
        }
    }

    public void CalculateScrollbarSensitivity()
    {
        if (objectiveCount <= 1) return;
        scrollbarSensitivity = 1f / (objectiveCount - 1);
    }

    void OnEnd()
    {
        Invoke("OnEnd2", 0.1f);
    }
    void OnEnd2()
    {
        scrollbar.value = 1f;
    }
}
