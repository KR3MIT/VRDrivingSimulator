using NUnit.Framework;
using UnityEngine;
//using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.XR;

public class ForceXRRigLocation : MonoBehaviour
{
    private Vector3 lockPosition;
    [SerializeField] private LogitechInput logitechInput;
    private Vector3 positionOffset;
    public float sensitivity;
    public Transform XRCamera;

    private float inputDelay = 0.2f;
    private float lastInputTime;
    public Vector3 targetRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lockPosition = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
       Inputs();
    }



    /// <summary>
    /// Recenters the XR tracking origin using available XRInputSubsystem instances.
    /// Also (optionally) applies the local transform lock position / rotation if you want the rig moved.
    /// Use this to reset the user's view orientation/position in XR.
    /// </summary>
    public void SetXRPosition()
    {
        // Recenter tracking using XRInputSubsystem(s)
        //List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();
        //SubsystemManager.GetSubsystems(inputSubsystems);

        //if (inputSubsystems.Count == 0)
        //{
        //    Debug.LogWarning("SetXRPosition: No XRInputSubsystem instances found. Make sure an XR loader is active.");
        //}
        //else
        //{
        //    foreach (var ss in inputSubsystems)
        //    {
        //        if (ss != null)
        //        {
        //            bool success = ss.TryRecenter();
        //            Debug.Log($"SetXRPosition: TryRecenter returned {success} for subsystem {ss.GetType().Name}");
        //        }
        //    }
        //}

        // If you want to physically move the rig after recentering, apply offsets here.
        // Uncomment the lines below if you want the GameObject with this script to be moved/rotated.
        lockPosition += positionOffset;
        this.transform.localPosition = lockPosition;
        //this.transform.localRotation = Quaternion.Euler(targetRotation);
    }

    /// <summary>
    /// Convenience public method to explicitly reset orientation only (if needed by callers/UI).
    /// </summary>
    public void ResetXRViewOrientation()
    {
        SetXRPosition();
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
                lastInputTime = Time.deltaTime;
                SetXRPosition();
            }
            else if (logitechInput.dpadValue == -1) // Down
            {
                ZOffset -= sensitivity;
                lastInputTime = Time.deltaTime;
                SetXRPosition();
            }
            else if (logitechInput.dpadValue == 2) // Right
            {
                XOffset += sensitivity;
                lastInputTime = Time.deltaTime;
                SetXRPosition();
            }
            else if (logitechInput.dpadValue == -2) // Left
            {
                XOffset -= sensitivity;
                lastInputTime = Time.deltaTime;
                SetXRPosition();
            }
            positionOffset = new Vector3(XOffset, 0f, ZOffset);
        }
    }

    //public void SetXRPosition()
    //{


    //    List<InputDevice> devices = new List<InputDevice>();
    //    InputDevices.GetDevices(devices);
    //    if(devices.Count != 0)
    //    {
    //        foreach (var device in devices)
    //        {
    //            device.subsystem.TryRecenter();
    //        }
    //    }
    //    Debug.Log("shitass " + devices);
    //    //lockPosition += positionOffset;
    //    //this.transform.localPosition = lockPosition;
    //    //this.transform.rotation = Quaternion.Euler(targetRotation);
    //}
}
