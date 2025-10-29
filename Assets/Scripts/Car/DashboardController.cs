using UnityEngine;

public class DashboardController : MonoBehaviour
{
    
    private LogitechInput logitechInput;
    private CarMover carMover;
    [Header("Speedometer")]
    [SerializeField]
    private float speed = 0f;
    [SerializeField]
    private float minRotation = 140f;
    [SerializeField]
    private float maxRotation = -125f;
    [SerializeField]
    private GameObject speedNeedlePivot;
    [SerializeField]
    private TMPro.TextMeshProUGUI speedDisplay;

    [Header("Blinkers")]
    public GameObject leftBlinkerLight;
    public GameObject rightBlinkerLight;
    [SerializeField]
    private float blinkerFlashRate = 0.5f;
    private float blinkerTimer = 0f;
    [SerializeField]
    private bool blinkerState = false;
    public float wheelRotationThreshold;
    [SerializeField]
    private float wheelRotation;

    //blinker bools
    private bool leftBlinkerOn = false;
    private bool rightBlinkerOn = false;
    private bool anyBlinkerOn = false;
    private bool prevLeftBlinker = false;
    private bool prevRightBlinker = false;
    private bool rightWheelRotationThresholdExceeded = false;
    private bool leftWheelRotationThresholdExceeded = false;

    // Animation of speed needle
    const float maxSpeed = 220f;
    float speedNormalized = 0f;
    float pivotRotation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logitechInput = GetComponent<LogitechInput>();
        carMover = GetComponent<CarMover>();
        speedNeedlePivot.transform.localRotation = Quaternion.Euler(0, 0, minRotation);
        leftBlinkerLight.SetActive(false);
        rightBlinkerLight.SetActive(false);
        wheelRotationThreshold = Mathf.Abs(wheelRotationThreshold);
    }

    // Update is called once per frame
    void Update()
    {
        AnimateSpeedNeedle();
        CheckBlinkerChanges();
        CheckWheelRotationCancelBlinkers();
        BlinkerControl();
    }

    private void BlinkerControl()
    {
        if (logitechInput == null) return;
  
        anyBlinkerOn = leftBlinkerOn || rightBlinkerOn;

        if (anyBlinkerOn)
        {
            blinkerTimer += Time.deltaTime;
            if (blinkerTimer >= blinkerFlashRate)
            {
                blinkerTimer -= blinkerFlashRate;
                blinkerState = !blinkerState;
            }
        }
        else
        {
            blinkerState = false;
            blinkerTimer = 0f;
        }

        if (rightBlinkerOn)
        {
            rightBlinkerLight.SetActive(blinkerState);
        }
        else
        {
            rightBlinkerLight.SetActive(false);
        }

        if (leftBlinkerOn)
        {
            leftBlinkerLight.SetActive(blinkerState);
        }
        else
        {
            leftBlinkerLight.SetActive(false);
        }
    }

    private void CheckBlinkerChanges ()
    {
        bool currentLeftBlinker = logitechInput.leftBlinker;
        bool currentRightBlinker = logitechInput.rightBlinker;

        if (currentLeftBlinker && !prevLeftBlinker)
        {
            ToggleLeftBlinker();
        }

        if (currentRightBlinker && !prevRightBlinker)
        {
            ToggleRightBlinker();
        }
        prevLeftBlinker = currentLeftBlinker;
        prevRightBlinker = currentRightBlinker;
    }

    public void ToggleRightBlinker()
    {
        rightBlinkerOn = !rightBlinkerOn;
        leftBlinkerOn = false;
        if (rightBlinkerOn)
        {
            blinkerState = true;
            blinkerTimer = 0f;
        }
        else
        {
            blinkerState = false;
            blinkerTimer = 0f;
            if (leftBlinkerLight != null) leftBlinkerLight.SetActive(false);
            if (rightBlinkerLight != null) rightBlinkerLight.SetActive(false);
        }
    }

    private void ToggleLeftBlinker()
    {
        leftBlinkerOn = !leftBlinkerOn;
        rightBlinkerOn = false;
        // start visible immediately when turned on
        if (leftBlinkerOn)
        {
            blinkerState = true;
            blinkerTimer = 0f;
        }
        else
        {
            blinkerState = false;
            blinkerTimer = 0f;
            if (leftBlinkerLight != null) leftBlinkerLight.SetActive(false);
            if (rightBlinkerLight != null) rightBlinkerLight.SetActive(false);
        }
    }

    private void CheckWheelRotationCancelBlinkers()
    {
        wheelRotation = carMover.wheelRotation;

        if (leftBlinkerOn)
        {
            if (!leftWheelRotationThresholdExceeded && wheelRotation < -wheelRotationThreshold)
            {
                leftWheelRotationThresholdExceeded = true;
            }
            if (leftWheelRotationThresholdExceeded && wheelRotation >= -wheelRotationThreshold)
            {
                leftWheelRotationThresholdExceeded = false;
                leftBlinkerOn = false;
                if (!rightBlinkerOn)
                {
                    blinkerState = false;
                    blinkerTimer = 0f;
                    leftBlinkerLight.SetActive(false);
                    rightBlinkerLight.SetActive(false);
                }
            }
        }
        else
        {
            leftWheelRotationThresholdExceeded = false;
        }

        if (rightBlinkerOn)
        {
            if (!rightWheelRotationThresholdExceeded && wheelRotation > wheelRotationThreshold)
            {
                rightWheelRotationThresholdExceeded = true;
            }
            if (rightWheelRotationThresholdExceeded && wheelRotation <= wheelRotationThreshold)
            {
                rightWheelRotationThresholdExceeded = false;
                rightBlinkerOn = false;
                if (!leftBlinkerOn)
                {
                    blinkerState = false;
                    blinkerTimer = 0f;
                    leftBlinkerLight.SetActive(false);
                    rightBlinkerLight.SetActive(false);
                }
            }
        }
        else
        {
            rightWheelRotationThresholdExceeded = false;
        }


    }
    private void AnimateSpeedNeedle()
    {
        speed = carMover.magnitude;
        speedNormalized = Mathf.Clamp(speed / maxSpeed, 0f, 1f);
        pivotRotation = Mathf.Lerp(minRotation, maxRotation, speedNormalized);
        speedNeedlePivot.transform.localRotation = Quaternion.Euler(0, 0, pivotRotation);

        if (speedDisplay != null)
        {
            speedDisplay.text = Mathf.RoundToInt(speed).ToString();
        }
    }

    public bool CheckLeftBlinkerOn()
    {
        return leftBlinkerOn;
    }

    public bool CheckRightBlinkerOn()
    {
        return rightBlinkerOn;
    }

}
