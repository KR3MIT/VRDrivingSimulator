using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{
    public enum TrafficLightState { Green, Yellow, Red, RedYellow}

    [Header("Initial States")]
    public TrafficLightState currentState1 = TrafficLightState.Red;
    public TrafficLightState currentState2 = TrafficLightState.Green;


    public List<GameObject> trafficLights1 = new List<GameObject>();
    public List<GameObject> trafficLights2 = new List<GameObject>();

    List<Material> mats1 = new List<Material>();
    List<Material> mats2 = new List<Material>();

    [Header("State Durations")]
    public float redTime = 24f;
    public float redYellowTime = 1.5f;
    public float greenTime = 17f;
    public float yellowTime = 3f;

    private float cycleTimer = 0f;
    private float totalCycle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // StartCoroutine(LightStateSwitch(1));
        //StartCoroutine(LightStateSwitch(2));
        //StartCoroutine(CoordinatedLightStateSwitch());
        GetMaterials();
        UpdateMaterialState();
        // compute full cycle length (one light’s total duration)
        totalCycle = redTime + redYellowTime + greenTime + yellowTime;
        StartCoroutine(CycleClock());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CycleClock()
    {
        while (true)
        {
            UpdateLights(cycleTimer);
            yield return null; // frame by frame for precision
            cycleTimer += Time.deltaTime;

            if (cycleTimer > totalCycle) cycleTimer = 0f;
        }
    }

    void UpdateLights(float t)
    {
        // Light 1 timing
        float redEnd = redTime;
        float redYellowEnd = redEnd + redYellowTime;
        float greenEnd = redYellowEnd + greenTime;
        float yellowEnd = greenEnd + yellowTime;

        // Wrap around
        float localT1 = t % yellowEnd;

        TrafficLightState state1;
        if (localT1 < redEnd) state1 = TrafficLightState.Red;
        else if (localT1 < redYellowEnd) state1 = TrafficLightState.RedYellow;
        else if (localT1 < greenEnd) state1 = TrafficLightState.Green;
        else state1 = TrafficLightState.Yellow;

        // Light 2 is opposite phase (half a cycle offset)
        float halfCycle = totalCycle / 2f;
        float localT2 = (t + halfCycle) % totalCycle;

        TrafficLightState state2;
        if (localT2 < redEnd) state2 = TrafficLightState.Red;
        else if (localT2 < redYellowEnd) state2 = TrafficLightState.RedYellow;
        else if (localT2 < greenEnd) state2 = TrafficLightState.Green;
        else state2 = TrafficLightState.Yellow;

        ApplyState(state1, state2);
    }

  
    void ApplyState(TrafficLightState s1, TrafficLightState s2)
    {
        foreach (var mat in mats1) mat.SetInt("_State", (int)s1);
        foreach (var mat in mats2) mat.SetInt("_State", (int)s2);
    }

   /* IEnumerator LightStateSwitch (int lightIndex)
    {
        while (true)
        {
            float waitTime = StateSelect(lightIndex);
            yield return new WaitForSeconds(waitTime);
            IncrementState(lightIndex);
            UpdateMaterialState();
            Debug.Log("Switching Traffic Lights" + currentState1 + " " + currentState2);

        }
    }
   */
   

    public TrafficLightState CheckLightState1()
    {
        return currentState1;
    }

    public TrafficLightState CheckLightState2()
    {
        return currentState2;
    }

   void IncrementState(int index)
    {
        if (index == 1) {
            currentState1 += 1;
            if ((int)currentState1 > 3) currentState1 = 0;
            return;
        }
        else if (index == 2) {
            currentState2 += 1;
            if ((int)currentState2 > 3) currentState2 = 0;
            return;
        }
    }
   void GetMaterials ()
    {

        foreach (var light in trafficLights1)
        {
            var renderer = light.GetComponent<Renderer>();
            if (renderer != null)
            {
                mats1.Add(renderer.material);
            }
        }
        foreach (var light in trafficLights2)
        {
            var renderer = light.GetComponent<Renderer>();
            if (renderer != null)
            {
                mats2.Add(renderer.material);
            }
        }
    }

   void UpdateMaterialState()
    {
        foreach (var mat in mats1)
        {
            if (mat != null)
            {
                mat.SetInt("_State", (int)currentState1);
            }
        }
        foreach (var mat in mats2)
        {
            if (mat != null)
            {
                mat.SetInt("_State", (int)currentState2);
            }
        }
    }

}
