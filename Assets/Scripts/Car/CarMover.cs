using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Windows;
using UnityEngine.InputSystem;

public class CarMover : MonoBehaviour
{
    private LogitechInput logitechInput;

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

    [Serializable]
    public struct Gear
    {
        public float ratio;
        public float upshiftSpeed;
        public float downshiftSpeed;
    }

    [Header("Keyboard or wheel")]
    public bool useWheelInput = true;


    [Header("Gear/Transmission")]
    public TransmissionType transmissionState = TransmissionType.Park;
    public bool useGears = true;
    public List<Gear> gears;
    public int currentGear = 0;

    [Header("Speeds")]
    public float maxAcceleration = 30.0f;
    public float reverseAcceleration = 20.0f;
    public float brakeAcceleration = 50.0f;
    public float creepTorque = 5.0f;
    public float maxCreepSpeed = 5.0f;
    public float minCreepSpeed = 0.5f;

    [Header("Brake")]
    public float brakeShiftThreshold = 0.25f;

    [Header("Steering")]
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    [Header("Rigidbody")]
    public Vector3 _centerOfMass;
    public bool freezeYPosition = true;

    [Header("Wheels")]
    public List<Wheel> wheels;

    [Header("Steering Wheel")]
    public GameObject steeringWheel;

    [Header("Engine")]
    public AnimationCurve engineTorqueCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float maxEngineRPM = 6000f;
    public float minEngineRPM = 800f;
    public float driveRatio = 4f;
    public float currentEngineRPM = 0f;

    [Header("values for debug")]
    public float acceleratorInput;
    public float brakeInput;
    public float steerInput;

    public float magnitude;

    private bool isCreeping = false;

    //priv value
    private float rawSteerInput;

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
        logitechInput = GetComponent<LogitechInput>();

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

        if (freezeYPosition)//make it impossible to flip, messes a bit with speed value ie slows down everything
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
        HandleAutomaticGears();
        AnimateWheels();
        AnimateSteeringWheel();

        magnitude = carRb.linearVelocity.magnitude;
    }

    void FixedUpdate()
    {
        Move();
        Steer();
        Brake();
    }

    void GetInputs()
    {
        if (useWheelInput)
        {
            rawSteerInput = (float)logitechInput.steeringWheelValue;
            float steering = rawSteerInput / 32768f;
            if(steering < .05 && steering > -.05) { steering = 0f;}
            steerInput = steering;

            float speeder = (float)logitechInput.speederValue;
            speeder = (speeder + 32768f) / 2 / 32768f;
            speeder = 1 - speeder;
            if(speeder < 0.05f) speeder = 0f;
            acceleratorInput = speeder;

            float brake = (float)logitechInput.brakeValue;
            brake = (brake + 32768f) / 2 / 32768f;
            brake = 1 - brake;
            if (brake < 0.05f) brake = 0f;
            brakeInput = brake;
            
        }
        else
        {
            acceleratorInput = acceleratorPedal.ReadValue<float>();
            steerInput = steer.ReadValue<float>();
            brakeInput = brakePedal.ReadValue<float>();
        }
    }

    void Move()
    {
        float torque = 0f;
        float speed = carRb.linearVelocity.magnitude;

        logitechInput.SetSpringForce(speed);

        float wheelRPM = 0f;

        if(transmissionState == TransmissionType.Drive && useGears)
        {
            foreach (var wheel in wheels)
            {
                wheelRPM += wheel.wheelCollider.rpm;
            }
            wheelRPM /= wheels.Count;//get average RPM of all wheels
        }

        if (transmissionState == TransmissionType.Drive)
        {
            float gearRatio = useGears ? gears[currentGear].ratio : 1f;

            //find rpm of engine based on wheel rpm and gear ratio
            currentEngineRPM = currentEngineRPM = Mathf.Abs(wheelRPM * gearRatio * driveRatio);
            currentEngineRPM = Mathf.Clamp(currentEngineRPM, minEngineRPM, maxEngineRPM);

            //get torque multiplier from engine torque curve
            float normalizedRPM = (currentEngineRPM - minEngineRPM) / (maxEngineRPM - minEngineRPM);
            float torqueMultiplier = engineTorqueCurve.Evaluate(normalizedRPM);

            if (acceleratorInput > .02f)
            {
                isCreeping = false;
                torque = acceleratorInput * maxAcceleration * gearRatio * torqueMultiplier;
            }else if (brakeInput < brakeShiftThreshold)//no gas
            {
                isCreeping = true;

                float creepValue = 1f - Mathf.Clamp01(brakeInput / brakeShiftThreshold);
                //Debug.Log("creep value: " + creepValue);

                torque = creepTorque * gearRatio * creepValue;

                var currentCreepSpeed = Mathf.Lerp(minCreepSpeed, maxCreepSpeed, creepValue);
                if (speed > currentCreepSpeed)
                {
                    torque = 0f;
                }

            }else
            {
                isCreeping = false;
            }
        }
        else if (transmissionState == TransmissionType.Reverse)
        {
            if (acceleratorInput > .01f)
            {
                torque = -acceleratorInput * reverseAcceleration;
            }
            else if (brakeInput < 0.01f && speed < maxCreepSpeed)//no gas or brake
            {
                torque = -creepTorque;
            }
        }//neutral and park = 0 torque




        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = torque;
        }
    }

    void Brake()
    {
        float brakeForce = 0f;

        //if in park should brake hard always
        if (transmissionState == TransmissionType.Park)
        {
            brakeForce = 10000f;
        }
        else if (!isCreeping)
        {
            brakeForce = brakeInput * brakeAcceleration;
        }

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.brakeTorque = brakeForce;
        }
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

        if (brakeInput > brakeShiftThreshold)
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

    void HandleAutomaticGears()
    {
        if(!useGears) return;

        if (transmissionState != TransmissionType.Drive)
        {
            currentGear = 0;
            return;
        }

        float speed = carRb.linearVelocity.magnitude;

        //upshift
        if (currentGear < gears.Count - 1 && speed > gears[currentGear].upshiftSpeed)
        {
            currentGear++;
        }//downsift
        else if (currentGear > 0 && speed < gears[currentGear].downshiftSpeed)
        {
            currentGear--;
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

    void AnimateSteeringWheel()
    {
        if (!steeringWheel) { return; }

        float normalizedSteer = Mathf.Clamp(rawSteerInput / 32768f, -1f, 1f);
        float wheelRotation = normalizedSteer * 450;
        steeringWheel.transform.localRotation = Quaternion.Euler(-wheelRotation, -90f, 0);
        //Debug.Log("wheelrot" + wheelRotation);
    }
}