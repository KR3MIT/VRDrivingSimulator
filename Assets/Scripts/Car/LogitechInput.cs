using Logitech;
using UnityEngine;

public class LogitechInput : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Values")]
    public int steeringWheelValue;
    public int speederValue;
    public int brakeValue;

    public bool leftBlinker;
    public bool rightBlinker;

    public bool SelectButtonA;
    public bool SelectButtonB;
    public bool StartButton;
    public bool RSB;
    public bool XboxButton;

    public int dpadValue; // center = -1, up = 0, right = 2, down = 3, left = 4

    [Header("heheheha")]
    public int baseSaturation = 40;
    public int baseCoefficient = 40;
    public int maxSaturation = 100;
    public int maxCoefficient = 100;
    public int speedCoefficient = 1;

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

            dpadValue = (int)rec.rgdwPOV[0];

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


            if (rec.rgbButtons[0] == 128)
            {
                SelectButtonA = true;
            }
            else { SelectButtonA = false; }

            if (rec.rgbButtons[1] == 128)
            {
                SelectButtonB = true;
            }
            else { SelectButtonB = false; }

            if (rec.rgbButtons[6] == 128)
            {
                StartButton = true;
            }
            else { StartButton = false; }

            if (rec.rgbButtons[8] == 128)
            {
                RSB = true;
            }
            else { RSB = false; }

            if(rec.rgbButtons[10] == 128)
            {
                XboxButton = true;
            }
            else { XboxButton = false; }


            switch (rec.rgdwPOV[0])
            {
                case (0): dpadValue = 1; break; //up
                //case (4500): actualState += "POV : UP-RIGHT\n"; break; //up-right
                case (9000): dpadValue = 2; break; //right
                //case (13500): actualState += "POV : DOWN-RIGHT\n"; break; //down-right
                case (18000): dpadValue = -1; break; //down
                //case (22500): actualState += "POV : DOWN-LEFT\n"; break; //down-left
                case (27000): dpadValue = -2; break; //left
                //case (31500): actualState += "POV : UP-LEFT\n"; break; //up-left
                default: dpadValue = 0; break; //center
            }
        }
    }

    public void SetSpringForce(float speed)
    {
        baseSaturation = 40;
        baseCoefficient = 40;
        maxSaturation = 100;
        maxCoefficient = 100;

        int saturation = Mathf.Clamp(baseSaturation + (int)(speed*speedCoefficient), baseSaturation, maxSaturation);
        int coefficient = Mathf.Clamp(baseCoefficient + (int)(speed*speedCoefficient), baseCoefficient, maxCoefficient);

        LogitechGSDK.LogiPlaySpringForce(0, 0, saturation, coefficient);
    }

    void OnApplicationQuit()
    {
        //Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
    }
}
