using UnityEngine;
using UnityEngine.InputSystem;

public class test : MonoBehaviour
{
    public PlayerInput input;
    private InputAction gasPedalAction;
    private InputAction steer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gasPedalAction = input.actions["GasPedal"];
        gasPedalAction.Enable();

        steer = input.actions["Steer"];
        steer.Enable();
    }

    private void Update()
    {
        if (steer != null)
        {
            float steerVal = steer.ReadValue<float>();
            Debug.Log("GasPedal value: " + steerVal);
        }
    }
}
