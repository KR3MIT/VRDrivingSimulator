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

    private int selectedButtonIndex = 0;
    private LogitechInput logitechInput;
    private float inputDelay = 0.2f; // Delay between input reads
    private float lastInputTime;

    private void Start()
    {
        logitechInput = GetComponent<LogitechInput>();
        SelectButton(selectedButtonIndex);

    }

    void Update()
    {
        HandleSteeringWheelInput();
    }

    void HandleSteeringWheelInput()
    {
       
        if (Time.time - lastInputTime > inputDelay)
        {
            if (logitechInput.dpadValue == 0) // Up
            {
                selectedButtonIndex = (selectedButtonIndex - 1 + menuButtons.Length) % menuButtons.Length;
                SelectButton(selectedButtonIndex);
                lastInputTime = Time.time;
            }
            else if (logitechInput.dpadValue == 1) // Down
            {
                selectedButtonIndex = (selectedButtonIndex + 1) % menuButtons.Length;
                SelectButton(selectedButtonIndex);
                lastInputTime = Time.time;
            }
        }

        // A button
        if (logitechInput.SelectButtonA)
        {
            menuButtons[selectedButtonIndex].onClick.Invoke();
        }
    }


    void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(menuButtons[index].gameObject);
    }


    public void StartButton()
    {
        mainMenu.SetActive(false);
        startMenu.SetActive(true);
    }

    public void ControlsButton()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }


    public void BackButton()
    {
        mainMenu.SetActive(true);
        startMenu.SetActive(false);
        controlsMenu.SetActive(false);
    }


    public void QuitButton()
    {
        Application.Quit();
    }

    //StartMenu Scenario Buttons
    



}
