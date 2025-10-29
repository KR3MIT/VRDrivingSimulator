using Logitech;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject startMenu;
    public GameObject controlsMenu;

    public Button[] menuButtons;
    public Button[] startButtons;
    public Button[] controlsButtons;


    private Button[] currentButtons;
    private int selectedButtonIndex = 0;
    private LogitechInput logitechInput;
    private float inputDelay = 0.2f; // Delay between input reads
    private float lastInputTime;

    private bool previousAButton = false;
    private bool previousBButton = false;

    private void Start()
    {
        logitechInput = GetComponent<LogitechInput>();
        currentButtons = menuButtons;
        selectedButtonIndex = 0;
        SelectButton(selectedButtonIndex);

    }

    void Update()
    {
        HandleSteeringWheelInput();
        Debug.Log("current index: " + selectedButtonIndex);
    }

    void HandleSteeringWheelInput()
    {
       
        if (Time.time - lastInputTime > inputDelay)
        {
            if (logitechInput.dpadValue == 1) // Up
            {
                selectedButtonIndex = (selectedButtonIndex - 1 + currentButtons.Length) % currentButtons.Length;
                SelectButton(selectedButtonIndex);
                lastInputTime = Time.time;
            }
            else if (logitechInput.dpadValue == -1) // Down
            {
                selectedButtonIndex = (selectedButtonIndex + 1) % currentButtons.Length;
                SelectButton(selectedButtonIndex);
                lastInputTime = Time.time;
            }

            // A button edge detection
            if (!previousAButton && logitechInput.SelectButtonA)
            {
                currentButtons[selectedButtonIndex].onClick.Invoke();
            }
            previousAButton = logitechInput.SelectButtonA;

            // B button edge detection
            if (!previousBButton && logitechInput.SelectButtonB)
            {
                BackButton();
            }
            previousBButton = logitechInput.SelectButtonB;
        }

       
    }


    void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(currentButtons[index].gameObject);
    }


    public void StartButton()
    {
        mainMenu.SetActive(false);
        startMenu.SetActive(true);
        currentButtons = startButtons;
        selectedButtonIndex = 0;
        SelectButton(selectedButtonIndex);
    }

    public void ControlsButton()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
        currentButtons = controlsButtons;
        selectedButtonIndex = 0;
        SelectButton(selectedButtonIndex);
    }


    public void BackButton()
    {
        mainMenu.SetActive(true);
        startMenu.SetActive(false);
        controlsMenu.SetActive(false);
        currentButtons = menuButtons;
        selectedButtonIndex = 0;
        SelectButton(selectedButtonIndex);
    }


    public void QuitButton()
    {
        //TEST
        FindFirstObjectByType<DataLog>().LogError(new DrivingError("Ran a red light", 12f, "Player ran red at intersection 3", DrivingError.ErrorSeverity.High));
        Application.Quit();
    }

    //StartMenu Scenario Buttons
    



}
