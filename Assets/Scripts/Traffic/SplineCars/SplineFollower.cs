using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{

    // THINGS TO DO IN THIS SCRIPT
    // 1. Make smooth speed transition when starting drive from a standstill - DONE
    // 2. Introduce some randomness to speed and acceleration - maybe not actually
    // 3. Implement system for handling other cars on the road (e.g., slowing down if a car in front is too close) - WIP
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

    Collider otherCarCol;
    bool isTooCloseToCarInFront = false;
    RaycastHit hit;
    float raycastDistance = 0f;
    float distanceToOtherCar = Mathf.Infinity;
    float distanceModifier = 3f; // Multiplier to increase the distance for raycasting based on speed



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
        CheckIfCarInFront();
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

        // Store the previous progress to maintain position on the spline
        float prevProgress = splineAnimate.NormalizedTime;
        float prevSpeed = currentSplineSpeed;

        if (isTooCloseToCarInFront)
        {
            float otherCarSpeed = otherCarCol.GetComponent<SplineFollower>().currentSplineSpeed;

            if (otherCarSpeed < 1f)
            {
                currentSplineSpeed = Mathf.Lerp(0.001f, currentSplineInfo.TraversalSpeed / 2, distanceToOtherCar - 5 / raycastDistance);
                currentSplineSpeed = Mathf.Min(currentSplineSpeed, prevSpeed);
            }
            else
            {
                currentSplineSpeed = Mathf.Min(currentSplineSpeed, Mathf.Lerp(otherCarSpeed * 0.95f, currentSplineSpeed, distanceToOtherCar / raycastDistance));
            }
                
            
            
            //currentSplineSpeed = Mathf.Lerp(otherCarCol.GetComponent<SplineFollower>().currentSplineSpeed * 0.95f, currentSplineSpeed, distanceToOtherCar / raycastDistance);

            //currentSplineSpeed = Mathf.Max(currentSplineSpeed, 0.001f); // Ensure speed doesn't go negative
        }
        else
        {

            // Calculate distance to the start of the next spline
            float distance = Vector3.Distance(transform.position, nextSplineStartPoint);

            nextSplineLocked = nextSplineInfo.isSplineLocked;

            // Determine the appropriate distance threshold based on whether the current spline is a turn
            float distanceThreshold = currentSplineIsATurn ? speedBlendDisOnTurn : speedBlendDisOffTurn;

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

                // Smoothly accelerate from 0 to currentSplineSpeed
                currentSplineSpeed = Mathf.Lerp(0, currentSplineInfo.TraversalSpeed, tAcc);

                if (tAcc >= 0.95f)
                    isStartingFromStandstill = false; // Finished accelerating from standstill
            }

            // If speed is less than 1 m/s, consider the car to be in a standstill, as long as it's not already starting from a standstill
            if (!isStartingFromStandstill) { isInStandstill = currentSplineSpeed < 1f; }
        }

        if (currentSplineSpeed - prevSpeed > Time.deltaTime * accelerationRate) 
        {
            currentSplineSpeed = prevSpeed + (accelerationRate * Time.deltaTime); // Limit acceleration rate to prevent sudden upward jumps in speed
        }


        // Set the spline animation speed and maintain progress on the spline
        splineAnimate.MaxSpeed = currentSplineSpeed;
        splineAnimate.NormalizedTime = prevProgress;
    }

    void CheckIfCarInFront()
    {

        // Adjust raycast distance based on current speed, meaning faster speeds will check further ahead
        // With a 3 multiplier, at 50 km/h (14 m/s) the raycast will check about 40 meters ahead
        raycastDistance = currentSplineSpeed * distanceModifier;

        raycastDistance = Mathf.Max(raycastDistance, 5f); // Ensure a minimum raycast distance of 5 meters

        // Perform a box cast forward from the car's position to detect other cars
        if (Physics.BoxCast(transform.position, transform.localScale, transform.forward, out hit, transform.rotation, raycastDistance))
        {
            if (hit.collider.CompareTag("SplineCar"))
            {
                otherCarCol = hit.collider;
                isTooCloseToCarInFront = true;
                Debug.Log("Car detected in front: " + otherCarCol.name);

                distanceToOtherCar = hit.distance;
            }
        }
        else
        {
            // If no car is detected in front, reset the flag
            isTooCloseToCarInFront = false;
            otherCarCol = null;
            distanceToOtherCar = Mathf.Infinity;
        }

    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (isTooCloseToCarInFront)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, transform.localScale);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * raycastDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * raycastDistance, transform.localScale);
        }
    }
}
