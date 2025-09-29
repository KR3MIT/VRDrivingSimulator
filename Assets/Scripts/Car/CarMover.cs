using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

public class CarMover : MonoBehaviour
{
    //todo: automatic transmission, braking behaviour, engine power curve, 

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    public enum TransmissionType
    {
        Park,
        Reverse,
        Neutral,
        Drive,
    }

    [Header("Gear/Transmission")]
    public TransmissionType transmissionState = TransmissionType.Park;

    [Header("Speeds")]
    public float maxAcceleration = 30.0f;
    public float reverseAcceleration = 20.0f;
    public float brakeAcceleration = 50.0f;

    [Header("Steering")]
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    [Header("Rigidbody")]
    public Vector3 _centerOfMass;

    [Header("Wheels")]
    public List<Wheel> wheels;

    [Header("Input values for debug")]
    public float acceleratorInput;
    public float brakeInput;
    public float steerInput;

    //inputs
    private PlayerInput input;
    private InputAction acceleratorPedal;
    private InputAction brakePedal;
    private InputAction steer;

    private InputAction shiftUp;
    private InputAction shiftDown;

    private Rigidbody carRb;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;

        input = GetComponent<PlayerInput>();

        acceleratorPedal = input.actions["GasPedal"];
        brakePedal = input.actions["BrakePedal"];
        steer = input.actions["Steer"];

        acceleratorPedal.Enable();
        brakePedal.Enable();
        steer.Enable();

        shiftUp = input.actions["ShiftUp"];
        shiftDown = input.actions["ShiftDown"];

        shiftUp.Enable();
        shiftDown.Enable();



        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.ConfigureVehicleSubsteps(5f, 12, 15);
        }

        Invoke(nameof(FreezePositionY), 3f);
    }

    void FreezePositionY()
    {
        carRb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    void Update()
    {
        GetInputs();
        HandleTransmission();
        AnimateWheels();

        if (shiftUp.triggered || shiftDown.triggered)
        {
            Debug.Log("Transmission: " + transmissionState);
        }
    }

    void FixedUpdate()
    {
        Move();
        Steer();
        Brake();
    }

    void GetInputs()
    {
        acceleratorInput = acceleratorPedal.ReadValue<float>();
        steerInput = steer.ReadValue<float>();
        brakeInput = brakePedal.ReadValue<float>();

    }

    void Move()
    {
        float torque = 0f;

        if (transmissionState == TransmissionType.Drive)
        {
            torque = acceleratorInput * maxAcceleration;
        }
        else if (transmissionState == TransmissionType.Reverse)
        {
            torque = -acceleratorInput * reverseAcceleration;
        }//neutral and park = 0 torque

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = torque;
        }

        Debug.Log("Motor Torque: " + torque);
    }

    void Brake()
    {
        float brakeForce = 0f;

        //if in park should brake hard always
        if (transmissionState == TransmissionType.Park)
        {
            brakeForce = 10000f;
        }
        else
        {
            brakeForce = brakeInput * brakeAcceleration;
        }

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.brakeTorque = brakeForce;
        }

        Debug.Log("Brake Force: " + brakeForce);
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void HandleTransmission()
    {
        float speed = carRb.linearVelocity.magnitude;

        if (brakeInput > .8f)
        {//only shift if brake is held
            if (shiftUp.triggered)
            {
                if (transmissionState == TransmissionType.Park)
                    transmissionState = TransmissionType.Reverse;
                else if (transmissionState == TransmissionType.Reverse && speed < .5f)
                    transmissionState = TransmissionType.Neutral;
                else if (transmissionState == TransmissionType.Neutral && speed < .5f)
                    transmissionState = TransmissionType.Drive;
            }
            else if (shiftDown.triggered)
            {
                if (transmissionState == TransmissionType.Drive && speed < .5f)
                    transmissionState = TransmissionType.Neutral;
                else if (transmissionState == TransmissionType.Neutral && speed < .5f)
                    transmissionState = TransmissionType.Reverse;
                else if (transmissionState == TransmissionType.Reverse)
                    transmissionState = TransmissionType.Park;
            }
        }
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    //void Brake()
    //{
    //    if (driftPedal.ReadValue<float>() > .95f || moveInput == 0)
    //    {
    //        foreach (var wheel in wheels)
    //        {
    //            wheel.wheelCollider.brakeTorque = brakeAcceleration;
    //        }
    //    }
    //    else
    //    {
    //        foreach (var wheel in wheels)
    //        {
    //            wheel.wheelCollider.brakeTorque = 0;
    //        }
    //    }
    //}
}