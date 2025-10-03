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

    public bool SelectButtonA;
    public bool StartButton;

    public int dpadValue; // center = -1, up = 0, right = 2, down = 3, left = 4

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

            if (rec.rgbButtons[6] == 128)
            {
                StartButton = true;
            }
            else { StartButton = false; }



            switch (rec.rgdwPOV[0])
            {
                case (0): dpadValue = 0; break; //up
                //case (4500): actualState += "POV : UP-RIGHT\n"; break; //up-right
                //case (9000): actualState += "POV : RIGHT\n"; break; //right
                //case (13500): actualState += "POV : DOWN-RIGHT\n"; break; //down-right
                case (18000): dpadValue += 1; break; //down
                //case (22500): actualState += "POV : DOWN-LEFT\n"; break; //down-left
                //case (27000): actualState += "POV : LEFT\n"; break; //left
                //case (31500): actualState += "POV : UP-LEFT\n"; break; //up-left
                default: dpadValue += -1; break; //center
            }
        }
    }

    void OnApplicationQuit()
    {
        //Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
    }
}
