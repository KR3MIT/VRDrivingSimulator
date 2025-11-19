using Logitech;
using TMPro;
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

    public TMP_InputField playerNameInput;

    private Button[] currentButtons;
    private int selectedButtonIndex = 0;
    public LogitechInput logitechInput;
    private float inputDelay = 0.2f; // Delay between input reads
    private float lastInputTime;

    private bool previousAButton = false;
    private bool previousBButton = false;

    private void Start()
    {
        currentButtons = menuButtons;
        selectedButtonIndex = 0;
        SelectButton(selectedButtonIndex);

        // Load previously saved player name into the input field (if any)
        if (playerNameInput != null)
        {
            playerNameInput.text = PlayerPrefs.GetString("PlayerName", string.Empty);
            // Optionally subscribe to value change events in code:
            playerNameInput.onEndEdit.AddListener(OnPlayerNameChanged);
        }
    }

    void Update()
    {
        HandleSteeringWheelInput();
        //Debug.Log("current index: " + selectedButtonIndex);
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
        Debug.Log("Selecting button: " + currentButtons[index].name);
        EventSystem.current.SetSelectedGameObject(currentButtons[index].gameObject);
    }

    public void OnPlayerNameChanged(string newName)
    {
        PlayerPrefs.SetString("PlayerName", newName ?? string.Empty);
        PlayerPrefs.Save();
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
        //FeedbackSystem.Instance.RegisterDrivingError("Ran a red light", "Player ran red at intersection 3", DrivingError.ErrorSeverity.High);
        FindFirstObjectByType<DataLog>().EnsureInitialized();
        Application.Quit();
    }

    public void GoToScenario(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    //StartMenu Scenario Buttons




}
