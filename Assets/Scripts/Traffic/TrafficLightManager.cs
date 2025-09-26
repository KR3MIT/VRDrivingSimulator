using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{
    public enum TrafficLightState { Red, RedYellow, Green, Yellow}
    public TrafficLightState currentState1 = TrafficLightState.Red;
    public TrafficLightState currentState2 = TrafficLightState.Green;

    public List<GameObject> trafficLights1 = new List<GameObject>();
    public List<GameObject> trafficLights2 = new List<GameObject>();

    float longWait = 20f;
    float shortWait = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LightStateSwitch());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


   IEnumerator LightStateSwitch ()
    {
        float waitTime = 0;
        while (true)
        {
            
            if (currentState1 == TrafficLightState.Red || currentState1 == TrafficLightState.Green)
            {
                waitTime = longWait;
            }
            else
            {
                waitTime = shortWait;
            }

            yield return new WaitForSeconds(waitTime);
            
            currentState1 += 1;
            currentState2 += 1;
            if ((int)currentState1 > 3) currentState1 = 0;
            if ((int)currentState2 > 3) currentState2 = 0;
            Debug.Log("Switching Traffic Lights" + currentState1 + " " + currentState2);

        }
        
    }

}
