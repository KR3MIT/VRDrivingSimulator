using UnityEngine;
using AK.Wwise;

public class DashboardController : MonoBehaviour
{
    
    private LogitechInput logitechInput;
    private CarMover carMover;

    [Header("Speedometer")]
    [SerializeField] private float speed = 0f;
    [SerializeField] private float minRotation = 140f;
    [SerializeField] private float maxRotation = -125f;
    [SerializeField] private GameObject speedNeedlePivot;
    [SerializeField] private TMPro.TextMeshProUGUI speedDisplay;

    [Header("Blinkers")]
    public GameObject leftBlinkerLight;
    public GameObject rightBlinkerLight;
    public AK.Wwise.Event blinkerOnSoundEvent;
    public AK.Wwise.Event blinkerOffSoundEvent;
    [SerializeField] private float blinkerFlashRate = 0.5f;
    [SerializeField] private bool blinkerState = false;
    [SerializeField] private float wheelRotationThreshold;

    // useful private variables for blinker logic
    private float blinkerTimer = 0f;
    private bool leftBlinkerOn = false;
    private bool rightBlinkerOn = false;
    // private bool anyBlinkerOn = false;
    private bool prevLeftBlinker = false;
    private bool prevRightBlinker = false;
    private bool rightWheelRotationThresholdExceeded = false;
    private bool leftWheelRotationThresholdExceeded = false;

    // Animation of speed needle
    const float maxSpeed = 220f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logitechInput = GetComponent<LogitechInput>();
        carMover = GetComponent<CarMover>();

        SetBlinkerLights(false, false);
        wheelRotationThreshold = Mathf.Abs(wheelRotationThreshold);

        if(speedNeedlePivot != null)
            speedNeedlePivot.transform.localRotation = Quaternion.Euler(0, 0, minRotation);
       
    }

    // Update is called once per frame
    void Update()
    {
        AnimateSpeedNeedle();
        CheckBlinkerChanges();
        UpdateBlinkerFlashing();
        HandleWheelAutoCancel();
    }

    #region Speedometer
    private void AnimateSpeedNeedle()
    {
        if(carMover == null) return;

        speed = carMover.magnitude * 3.6f; // convert m/s to km/h
        float speedNormalized = Mathf.Clamp(speed / maxSpeed, 0f, 1f);
        float pivotRotation = Mathf.Lerp(minRotation, maxRotation, speedNormalized);
        
        if (speedNeedlePivot != null)
            speedNeedlePivot.transform.localRotation = Quaternion.Euler(0, 0, pivotRotation);
        else 
        Debug.LogWarning($"{nameof(DashboardController)}: Speed needle pivot is not assigned.");

        if (speedDisplay != null) speedDisplay.text = Mathf.RoundToInt(speed).ToString();
    }
    #endregion

    #region Blinkers
    private void CheckBlinkerChanges ()
    {
        
            if (logitechInput == null) return;

            bool curLeft = logitechInput.leftBlinker;
            bool curRight = logitechInput.rightBlinker;

            if (curLeft && !prevLeftBlinker) ToggleBlinker(ref leftBlinkerOn, ref rightBlinkerOn);
            if (curRight && !prevRightBlinker) ToggleBlinker(ref rightBlinkerOn, ref leftBlinkerOn);

            prevLeftBlinker = curLeft;
            prevRightBlinker = curRight;
    }

    private  void UpdateBlinkerFlashing()
    {
        if (!(leftBlinkerOn || rightBlinkerOn))
        {
            blinkerState = false;
            blinkerTimer = 0f;
            return;
        }

        blinkerTimer += Time.deltaTime;
        if (blinkerTimer < blinkerFlashRate) return;

        blinkerTimer -= blinkerFlashRate;
        blinkerState = !blinkerState;

        SetBlinkerLights(leftBlinkerOn && blinkerState, rightBlinkerOn && blinkerState);

        var soundEvent = blinkerState ? blinkerOnSoundEvent : blinkerOffSoundEvent;
        soundEvent.Post(gameObject);
    }

    private void ToggleBlinker(ref bool targetBlinker, ref bool oppositeBlinker)
    {
        targetBlinker = !targetBlinker;
        oppositeBlinker = false;
        blinkerState = targetBlinker;
        blinkerTimer = 0f;

        if (!targetBlinker)
            SetBlinkerLights(false, false);
    }

    private void HandleWheelAutoCancel()
    {
        float rotation = carMover.wheelRotation;

        HandleBlinkerAutoCancel(ref leftBlinkerOn, ref leftWheelRotationThresholdExceeded, rotation, true);
        HandleBlinkerAutoCancel(ref rightBlinkerOn, ref rightWheelRotationThresholdExceeded, rotation, false);
    }

    /// <summary>
    /// Handles automatic cancellation of a blinker after a wheel turn.
    /// </summary>
    /// <param name="blinkerOn">Reference to the blinker state (left or right)</param>
    /// <param name="thresholdExceeded">Reference to the threshold memory for this blinker</param>
    /// <param name="rotation">Current wheel rotation</param>
    /// <param name="isLeft">True if left blinker, false if right</param>
    private void HandleBlinkerAutoCancel(ref bool blinkerOn, ref bool thresholdExceeded, float rotation, bool isLeft)
    {
        if (blinkerOn)
        {
            // Mark threshold as exceeded when turning past it
            if (!thresholdExceeded && ((isLeft && rotation < -wheelRotationThreshold) || (!isLeft && rotation > wheelRotationThreshold)))
            {
                thresholdExceeded = true;
            }

            // Cancel blinker when wheel returns past threshold
            if (thresholdExceeded && ((isLeft && rotation >= -wheelRotationThreshold) || (!isLeft && rotation <= wheelRotationThreshold)))
            {
                blinkerOn = false;
                thresholdExceeded = false;

                // Turn off blinkers visually if none are active
                if (!leftBlinkerOn && !rightBlinkerOn)
                {
                    blinkerState = false;
                    blinkerTimer = 0f;
                    SetBlinkerLights(false, false);
                }
            }
        }
        else
        {
            thresholdExceeded = false;
        }
    }

    private void SetBlinkerLights(bool leftOn, bool rightOn)
    {
        if (leftBlinkerLight != null) leftBlinkerLight.SetActive(leftOn);
        if (rightBlinkerLight != null) rightBlinkerLight.SetActive(rightOn);
    }

    public bool CheckLeftBlinkerOn() => leftBlinkerOn;
    public bool CheckRightBlinkerOn() => rightBlinkerOn;
    #endregion

#region Deprecated, possibly useful code
#if false
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

                if (rightBlinkerOn)
                {
                    rightBlinkerLight.SetActive(blinkerState);
                    blinkerOnSoundEvent.Post(gameObject);
                }
                else
                {
                    rightBlinkerLight.SetActive(false);
                    blinkerOffSoundEvent.Post(gameObject);
                }

                if (leftBlinkerOn)
                {
                    leftBlinkerLight.SetActive(blinkerState);
                    blinkerOnSoundEvent.Post(gameObject);
                }
                else
                {
                    leftBlinkerLight.SetActive(false);
                    blinkerOffSoundEvent.Post(gameObject);
                }

            }
        }
        else
        {
            blinkerState = false;
            blinkerTimer = 0f;
        }

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
#endif
#endregion

}