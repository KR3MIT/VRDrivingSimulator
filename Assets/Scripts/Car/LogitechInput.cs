using Logitech;
using UnityEngine;

public class LogitechInput : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int steeringWheelValue;
    public int speederValue;
    public int brakeValue;

    public bool leftBlinker;
    public bool rightBlinker;

    void Start()
    {
        LogitechGSDK.LogiSteeringInitialize(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);

            steeringWheelValue = rec.lX;
            speederValue = rec.lY;
            brakeValue = rec.lRz;

            if (rec.rgbButtons[4] == 128)
            {
                rightBlinker = true;
            }
            else { rightBlinker = false;}

            if (rec.rgbButtons[5] == 128)
            {
                leftBlinker = true;
            }
            else { leftBlinker = false;}


        }
    }

    void OnApplicationQuit()
    {
        //Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
    }
}
