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

    List<Material> materialsLights1 = new List<Material>();
    List<Material> materialsLights2 = new List<Material>();

    public float redWait = 24f;
    public float redYellowWait = 2f;
    public float greenWait = 17f;
    public float yellowWait = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LightStateSwitch(currentState1));
        StartCoroutine(LightStateSwitch(currentState2));
        GetMaterials();
        UpdateMaterialState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


   IEnumerator LightStateSwitch (TrafficLightState currentState)
    {
        float waitTime = 0;
        while (true)
        {
            
            if (currentState == TrafficLightState.Red)
            {
                waitTime = redWait;
            }
            else if(currentState == TrafficLightState.RedYellow)
            {
                waitTime = redYellowWait;
            }
            else if (currentState == TrafficLightState.Green)
            {
                waitTime = greenWait;
            }
            else if (currentState == TrafficLightState.Yellow)
            {
                waitTime = yellowWait;
            }

            yield return new WaitForSeconds(waitTime);
            
            IncrementState();
            UpdateMaterialState();
            Debug.Log("Switching Traffic Lights" + currentState1 + " " + currentState2);

        }
   }
    public TrafficLightState CheckLightState1()
    {
        return currentState1;
    }

    public TrafficLightState CheckLightState2()
    {
        return currentState2;
    }

   void IncrementState()
    {
        currentState1 += 1;
        currentState2 += 1;
        if ((int)currentState1 > 3) currentState1 = 0;
        if ((int)currentState2 > 3) currentState2 = 0;
    }
   void GetMaterials ()
    {

        foreach (var light in trafficLights1)
        {
            var renderer = light.GetComponent<Renderer>();
            if (renderer != null)
            {
                materialsLights1.Add(renderer.material);
            }
        }
        foreach (var light in trafficLights2)
        {
            var renderer = light.GetComponent<Renderer>();
            if (renderer != null)
            {
                materialsLights2.Add(renderer.material);
            }
        }
    }

   void UpdateMaterialState()
    {
        foreach (var mat in materialsLights1)
        {
            if (mat != null)
            {
                mat.SetInt("_State", (int)currentState1);
            }
        }
        foreach (var mat in materialsLights2)
        {
            if (mat != null)
            {
                mat.SetInt("_State", (int)currentState2);
            }
        }
    }

}
