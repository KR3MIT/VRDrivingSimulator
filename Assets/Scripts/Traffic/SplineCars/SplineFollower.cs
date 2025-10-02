using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{

    // THINGS TO DO IN THIS SCRIPT
    // 1. Make smooth speed transition when starting drive from a standstill
    // 2. Introduce some randomness to speed and acceleration - maybe not actually
    // 3. Implement system for handling other cars on the road (e.g., slowing down if a car in front is too close)
    // 4. Implement system for instantiating and despawning cars (might not be in this script)




    [SerializeField] private List<SplineContainer> splines;

    int currentSpline = -1;
    SplineInfo currentSplineInfo;
    float currentSplineSpeed;
    bool currentSplineIsATurn;
    SplineInfo nextSplineInfo;
    float nextSplineSpeed;
    Vector3 nextSplineStartPoint;
    bool onLastSpline = false;
    bool nextSplineLocked;
   
    bool isInStandstill = false;
    bool isStartingFromStandstill = false;
    float startAccTime = 0f;
    float accelerationRate = 3f; // m/s^2

    float speedBlendDisOnTurn = 10f; // Distance over which to blend speed when on a turn spline going to a straight spline
    float speedBlendDisOffTurn = 50f; // Distance over which to blend speed when on a straight spline going to a turn spline

    SplineAnimate splineAnimate;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
    }

    // Update is called once per frame
    void Update()
    {
        if (splineAnimate.IsPlaying == false && !nextSplineLocked)
        {
            SwitchSpline();
        }

        AdjustCarSpeed();
    }

    void SwitchSpline()
    {
        if (onLastSpline) {
            Destroy(gameObject);
            return;
        }
        currentSpline++;
        splineAnimate.Container = splines[currentSpline];
        currentSplineInfo = splines[currentSpline].GetComponent<SplineInfo>();
        currentSplineIsATurn = currentSplineInfo.IsATurn;

        if (currentSpline < splines.Count - 1)
        {
            nextSplineInfo = splines[currentSpline + 1].GetComponent<SplineInfo>();
            nextSplineSpeed = nextSplineInfo.TraversalSpeed;
            nextSplineStartPoint = nextSplineInfo.SplineStartPoint;
            nextSplineLocked = nextSplineInfo.isSplineLocked;
        }
        else
        {
            onLastSpline = true;
        }
        AdjustCarSpeed();
        splineAnimate.Restart(true);
    }

    private void AdjustCarSpeed()
    {
        // Calculate distance to the start of the next spline
        float distance = Vector3.Distance(transform.position, nextSplineStartPoint);
        
        nextSplineLocked = nextSplineInfo.isSplineLocked;

        // Determine the appropriate distance threshold based on whether the current spline is a turn
        float distanceThreshold = currentSplineIsATurn ? speedBlendDisOnTurn : speedBlendDisOffTurn;

        // Store the previous progress to maintain position on the spline
        float prevProgress = splineAnimate.NormalizedTime;
        // Calculate interpolation factor for lerp speed adjustment
        float t = 1f - (distance / distanceThreshold);

        if (isInStandstill && !nextSplineLocked)
        {
            isStartingFromStandstill = true;
            isInStandstill = false;
            startAccTime = 0f;
        }


        // If within the distance threshold, start modifying speed. Otherwise, use the current spline's speed value.
        if (distance < distanceThreshold && !isStartingFromStandstill)
        {
            // If the next spline is locked, target speed is 0, else it's the next spline's speed
            float speedTarget = nextSplineLocked ? 0 : nextSplineSpeed;

            // Smoothly interpolate current speed towards target speed
            currentSplineSpeed = Mathf.Lerp(currentSplineInfo.TraversalSpeed, speedTarget, t);

        }
        else
        {
            currentSplineSpeed = currentSplineInfo.TraversalSpeed;
        }

        if (isStartingFromStandstill)
        {
            // Increment the time spent accelerating
            startAccTime += Time.deltaTime;
            // Calculate the time factor for acceleration, ensuring it doesn't exceed 1
            float tAcc = (startAccTime * accelerationRate) / currentSplineInfo.TraversalSpeed;
            // Clamp tAcc to a maximum of 1
            tAcc = Mathf.Clamp01(tAcc);

            Debug.Log("tAcc: " + tAcc);

            // Smoothly accelerate from 0 to currentSplineSpeed
            currentSplineSpeed = Mathf.Lerp(0, currentSplineInfo.TraversalSpeed, tAcc);

            if (tAcc >= 0.95f)
                isStartingFromStandstill = false; // Finished accelerating from standstill
        }


        // If speed is less than 1 m/s, consider the car to be in a standstill, as long as it's not already starting from a standstill
        if (!isStartingFromStandstill) {isInStandstill = currentSplineSpeed < 1f;}

        // Set the spline animation speed and maintain progress on the spline
        splineAnimate.MaxSpeed = currentSplineSpeed;
        splineAnimate.NormalizedTime = prevProgress;
    }

}
