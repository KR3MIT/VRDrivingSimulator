using UnityEngine;

public class ForceXRRigLocation : MonoBehaviour
{
    private Vector3 lockPosition;
    [SerializeField] private LogitechInput logitechInput;
    private Vector3 positionOffset;
    public float sensitivity;

    private float inputDelay = 0.2f;
    private float lastInputTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lockPosition = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        SetXRPosition();
        Inputs();
    }

    void Inputs()
    {
        var XOffset = 0f;
        var ZOffset = 0f;

        if (Time.time - lastInputTime > inputDelay)
        {
            //up = 1, down = -1, right = 2, left = -2
            if (logitechInput.dpadValue == 1) // Up
            {
                ZOffset += sensitivity;
                lastInputTime = Time.time;
            }
            else if (logitechInput.dpadValue == -1) // Down
            {
                ZOffset -= sensitivity;
                lastInputTime = Time.time;
            }
            else if (logitechInput.dpadValue == 2) // Right
            {
                XOffset += sensitivity;
                lastInputTime = Time.time;
            }
            else if (logitechInput.dpadValue == -2) // Left
            {
                XOffset -= sensitivity;
                lastInputTime = Time.time;
            }
            positionOffset = new Vector3(XOffset, 0f, ZOffset);
        }
    }

    void SetXRPosition()
    {
        lockPosition += positionOffset;
        this.transform.localPosition = lockPosition;
    }
}
