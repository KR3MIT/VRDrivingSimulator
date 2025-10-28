using UnityEngine;

public class DashboardController : MonoBehaviour
{
    
    private LogitechInput logitechInput;
    private CarMover carMover;
    [Header("Speedometer")]
    [SerializeField]
    private float speed = 0f;
    [SerializeField]
    private float minRotation = 133f;
    [SerializeField]
    private float maxRotation = -133f;
    [SerializeField]
    private GameObject speedNeedlePivot;

    [Header("Blinkers")]
    
    public GameObject leftBlinkerLight;
    public GameObject rightBlinkerLight;
    [SerializeField]
    private float blinkerFlashRate = 0.5f;
    [SerializeField]
    private float blinkerTimer = 0f;
    [SerializeField]
    private bool blinkerState = false;

    private bool leftBlinkerOn = false;
    private bool rightBlinkerOn = false;
    private bool anyBlinkerOn = false;

    private bool prevLeftBlinker = false;
    private bool prevRightBlinker = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logitechInput = GetComponent<LogitechInput>();
        carMover = GetComponent<CarMover>();
        speedNeedlePivot.transform.localRotation = Quaternion.Euler(0, 0, minRotation);
        leftBlinkerLight.SetActive(false);
        rightBlinkerLight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        AnimateSpeedNeedle();
        CheckBlinkerChanges();
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
            ToggleLeftBlinker();
        }
        prevLeftBlinker = currentLeftBlinker;
        prevRightBlinker = currentRightBlinker;
    }

    public void ToggleRightBlinker()
    {
        rightBlinkerOn = !rightBlinkerOn;
        if (leftBlinkerOn || rightBlinkerOn)
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
        // start visible immediately when turned on
        if (leftBlinkerOn || rightBlinkerOn)
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

    private void AnimateSpeedNeedle()
    {
        speed = carMover.magnitude;
       // float normalizeRotation = Mathf.Clamp(speed/220f,minRotation, maxRotation);
        float normalizeRotation = Mathf.Lerp(minRotation, maxRotation, speed / 220f);
        speedNeedlePivot.transform.localRotation = Quaternion.Euler(0, 0, normalizeRotation);
    }


}
