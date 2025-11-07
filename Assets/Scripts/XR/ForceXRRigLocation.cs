using NUnit.Framework;
using UnityEngine;
//using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.XR;
using System.ComponentModel.Design.Serialization;
//using UnityEngine.InputSystem;

public class ForceXRRigLocation : MonoBehaviour
{
    [SerializeField] private LogitechInput logitechInput;
    public Vector3 defaultPosition;
    public Transform steeringWheelCenter, leftHand, rightHand;
    public Vector3 posOff;

    bool isCooldown = false;

    private void Start()
    {
        defaultPosition = transform.localPosition;
    }

    void Update()
    {
        if (isCooldown) { return; }

        if ((logitechInput.leftBlinker || logitechInput.rightBlinker) || transform.root.GetComponent<UnityEngine.InputSystem.PlayerInput>().actions["Test"].ReadValue<float>() >= 1)
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


            }
        }

        StartCooldown();
    }

    public async void StartCooldown()
    {
        isCooldown = true;
        await System.Threading.Tasks.Task.Delay(1000);
        isCooldown = false;
    }
}
