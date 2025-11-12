using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR;


public class ForceXRRigLocation : MonoBehaviour
{
    [SerializeField] private LogitechInput logitechInput;
    public Vector3 defaultPosition;
    public Transform steeringWheelCenter, leftHand, rightHand;
    public Vector3 posOff;

    bool isCooldown = false;

    //save calibration 
    public static Vector3 SavedPosition = Vector3.zero;
    public static Quaternion SavedRotation = Quaternion.identity;
    public static bool HasSavedCalibration = false;

    public static event System.Action OnCalibrationDone;
    private static bool calibrated = false;

    private void Awake()
    {
        ApplySavedCalibration(transform);
    }

    private void Start()
    {
        defaultPosition = transform.localPosition;
    }

    void Update()
    {
        if (isCooldown) { return; }
        if (calibrated) { return; }

        if (logitechInput.rightBlinker)
        {
            OnCalibrationDone?.Invoke();
            calibrated = true;
        }

        if ((logitechInput.leftBlinker) || transform.root.GetComponent<UnityEngine.InputSystem.PlayerInput>().actions["Test"].ReadValue<float>() >= 1)
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.Head, devices);
            if (devices.Count > 0)
            {
                InputDevice headDevice = devices[0];
                if (headDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePosition))
                {
                    Debug.Log("Head Position: " + devicePosition);
                }

                // Calculate the forward direction based on the hands
                Vector3 handForward = (rightHand.position - leftHand.position).normalized;

                // Calculate the desired forward direction (steering wheel forward direction)
                Vector3 steeringForward = -steeringWheelCenter.forward;

                // Calculate the rotation needed to align handForward with steeringForward
                Quaternion rotationOffset = Quaternion.FromToRotation(handForward, steeringForward);

                // Extract the current rotation's Euler angles
                Vector3 currentEulerAngles = transform.rotation.eulerAngles;

                // Apply the rotation offset and isolate the y-axis rotation
                Quaternion newRotation = rotationOffset * transform.rotation;
                Vector3 newEulerAngles = newRotation.eulerAngles;

                // Preserve the x and z rotations, only update the y-axis
                transform.rotation = Quaternion.Euler(currentEulerAngles.x, newEulerAngles.y, currentEulerAngles.z);
                Debug.Log("XR Rig rotation adjusted to align hand direction with steering wheel direction (y-axis only).");



                // Calculate the center of the hands
                Vector3 handCenter = (leftHand.position + rightHand.position) / 2;
                Debug.Log("Hand Center: " + handCenter);

                // Calculate the position offset to align handCenter with steeringWheelCenter
                Vector3 positionOffset = steeringWheelCenter.position - handCenter;

                // Apply the position offset to the rig
                transform.position += positionOffset + posOff;
                Debug.Log("XR Rig position adjusted to align hand center with steering wheel center.");


                //save calibration
                SavedPosition = transform.localPosition;//maybe local instead?!
                SavedRotation = transform.localRotation;
                HasSavedCalibration = true;
                Debug.Log($"saved values= :) pos={SavedPosition} AND rot={SavedRotation.eulerAngles}");
            }
        }
        isCooldown = true;
        Invoke("StartCooldown", .5f);
    }

    void StartCooldown()
    {
        isCooldown = false;
    }

    public static void ApplySavedCalibration(Transform rigTransform)
    {
        if (!HasSavedCalibration) return;
        Debug.Log($"applying saved values= :) pos={SavedPosition} AND rot={SavedRotation.eulerAngles}");
        rigTransform.localPosition = SavedPosition;
        rigTransform.localRotation = SavedRotation;
    }
}
