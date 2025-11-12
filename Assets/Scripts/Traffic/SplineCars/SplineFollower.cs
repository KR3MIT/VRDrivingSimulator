using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{

    // THINGS TO DO IN THIS SCRIPT
    // 1. Make smooth speed transition when starting drive from a standstill - DONE
    // 2. Introduce some randomness to speed and acceleration - maybe not actually
    // 3. Implement system for handling other cars on the road (e.g., slowing down if a car in front is too close) - THIS IS DONE NOW!!! (i think)
    // 4. Implement system for instantiating and despawning cars (might not be in this script)
    // 5. Implement system for handling traffic lights being turned off too late to stop at the light - WIP 


    [Header("Pedestrian Spotting")]
    [Tooltip("Pedestrian Spotting FOV")]
    public float viewRadius = 10f;
    [Tooltip("Field of view angle (degrees)")]
    [Range(0f, 360f)]
    public float viewAngle = 90f;
    private string pedestrianTag = "Pedestrian";

    [Header("Visualization of pedestrian spotting")]
    [Tooltip("Number of segments used to draw the FOV arc. Higher gives smoother arc.")]
    [Range(4, 128)]
    public int meshResolution = 24;
    [Tooltip("Color used to draw the FOV lines")]
    public Color viewColor = new Color(0f, 1f, 0f, 0.25f);

    [Header("Spline Following")]
    [Tooltip("Splines for the car to follow (this is most likely set in code from spawner)")]
    public List<SplineContainer> splines;

    [Header("Blinker")]
    public Material frontBlinkingMaterial; 
    public Material rearBlinkingMaterial;
    public Material frontNormalMaterial, RearNormalMaterial;
    public Renderer rearLeftBlinker, rearRightBlinker, frontLeftBlinker, frontRightBlinker;

    private SplineRoute splineRoute;

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
    float minSafeDistance = 5f; // Minimum safe distance to maintain from the car in front
    float distanceModifier = 3f; // Multiplier to increase the distance for raycasting based on speed

    bool isPedestrianInFront = false;
    float distanceToPedestrian = Mathf.Infinity;

    float maxDistanceForStoppingAtRedLight = 20f; // Maximum distance at which the car will consider stopping for a red light
    bool wasRedLightOnBefore =  false; // Track if the red light was on in the previous frame


    SplineAnimate splineAnimate;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();

        //splines = splineRoute.splines; // Assign the splines from the SplineRoute (This is now done in the spawner)
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
        BlinkerLights();
        if (currentSplineIsATurn)
        {
            CheckIfPedestrianInFront();
        }
        else
        {
            isPedestrianInFront = false;
            distanceToPedestrian = Mathf.Infinity;
        }

    }

    void BlinkerLights()
    {
        return; //fix jay
        if(nextSplineInfo.IsATurn && Vector3.Distance(transform.position, nextSplineStartPoint) < 10f)
        {
            frontRightBlinker.material = frontBlinkingMaterial;
            rearRightBlinker.material = rearBlinkingMaterial;
        }
        else
        {
            frontRightBlinker.material = frontNormalMaterial;
            rearRightBlinker.material = RearNormalMaterial;
        }
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
        if (isPedestrianInFront)
        {
            //Debug.Log("Pedestrian detected in front at distance: " + distanceToPedestrian);
            float tPed = Mathf.Clamp01((distanceToPedestrian - 5) / viewRadius);
            currentSplineSpeed = Mathf.Lerp(0.001f, currentSplineInfo.TraversalSpeed, tPed);
        }
        else if (isTooCloseToCarInFront)
        {
            
            if (otherCarCol == null)
            {
                isTooCloseToCarInFront = false;
                return;
            }
            
            float otherCarSpeed;

            if (otherCarCol.TryGetComponent<SplineFollower>(out SplineFollower otherCarSplineFollower))
            {
               otherCarSpeed = otherCarSplineFollower.currentSplineSpeed;
            } else
            {
               otherCarSpeed = otherCarCol.GetComponent<CarMover>().magnitude * 3.6f;
            }
            
            //float otherCarSpeed = otherCarCol.GetComponent<SplineFollower>().currentSplineSpeed;
            //Debug.Log("Raycast distance: " + raycastDistance);
            //Debug.Log("Distance to other car: " + distanceToOtherCar);

            if (otherCarSpeed < 1f)
            {
                float adjustedRayDistance = Mathf.Max(raycastDistance - 5f, 5f);
                //Debug.Log("Adjusted ray distance: " + adjustedRayDistance);
                currentSplineSpeed = Mathf.Lerp(0.001f, currentSplineInfo.TraversalSpeed / 2, (distanceToOtherCar - minSafeDistance) / adjustedRayDistance);
                currentSplineSpeed = Mathf.Min(currentSplineSpeed, prevSpeed);
            }
            else
            {
                currentSplineSpeed = Mathf.Min(currentSplineSpeed, Mathf.Lerp(otherCarSpeed * 0.95f, currentSplineSpeed, distanceToOtherCar / raycastDistance));
            }
                
        }
        else
        {

            // Calculate distance to the start of the next spline
            float distance = Vector3.Distance(transform.position, nextSplineStartPoint);

            nextSplineLocked = nextSplineInfo.isSplineLocked;

            // If the next spline is locked and the red light wasn't on in the previous frame, check distance to decide if we should stop
            // If the car is too close to the red light, don't stop abruptly, just continue driving
            if (nextSplineLocked && !wasRedLightOnBefore)
            {
                if (distance > maxDistanceForStoppingAtRedLight)
                {
                    wasRedLightOnBefore = true;
                }
                else
                {
                    wasRedLightOnBefore = false;
                    nextSplineLocked = false;
                }
            }

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
            if (hit.collider.CompareTag("SplineCar") || hit.collider.CompareTag("Player"))
            {
                otherCarCol = hit.collider;
                isTooCloseToCarInFront = true;
                //Debug.Log("Car detected in front: " + otherCarCol.name);

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

    void CheckIfPedestrianInFront()
    {
        // Find candidate colliders in radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, LayerMask.GetMask("Default"));
        float bestDist = float.MaxValue;

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            if (!targetsInViewRadius[i].CompareTag(pedestrianTag))
                continue;

            if (targetsInViewRadius[i].gameObject.GetComponent<WaypointNavigation>() == null)
                continue;

            if (!targetsInViewRadius[i].gameObject.GetComponent<WaypointNavigation>().isCurrentlyCrossingRoad)
                continue;

            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // angle check
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle * 0.5f)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                // prefer the nearest visible target
                if (dstToTarget < bestDist)
                {
                    bestDist = dstToTarget;
                }
            }
        }

        if (bestDist < 1000) {
            isPedestrianInFront = true;
            distanceToPedestrian = bestDist;
        }
        else
        {
            isPedestrianInFront = false;
            distanceToPedestrian = Mathf.Infinity;
        }

    }


    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {

        // Draw field-of-view wedge if requested
        if (currentSplineIsATurn)
        {
            // Save previous color
            Color prevGiz = Gizmos.color;

            // Use a slightly darker line color for edges while keeping alpha
            Gizmos.color = viewColor;

            Vector3 origin = transform.position;
            Vector3 up = transform.up;
            Vector3 forward = transform.forward;

            int segments = Mathf.Clamp(meshResolution, 4, 128);
            float halfAngle = viewAngle * 0.5f;

            // Precompute the first point
            Vector3 prevPoint = origin + (Quaternion.AngleAxis(-halfAngle, up) * forward).normalized * viewRadius;
            // Draw line from origin to first boundary point
            Gizmos.DrawLine(origin, prevPoint);

            // Iterate segments and draw edges
            for (int i = 1; i <= segments; i++)
            {
                float t = (float)i / segments;
                float angle = -halfAngle + (viewAngle * t);
                Vector3 dir = Quaternion.AngleAxis(angle, up) * forward;
                Vector3 point = origin + dir.normalized * viewRadius;

                // Draw boundary edge segment
                Gizmos.DrawLine(prevPoint, point);
                // Draw line from origin to this boundary point (helps visualize wedge)
                Gizmos.DrawLine(origin, point);

                prevPoint = point;
            }

#if UNITY_EDITOR
            // Draw a filled arc in the Scene view using Handles (editor only).
            // Use UnityEditor.Handles.DrawSolidArc to respect alpha in viewColor.
            try
            {
                var prevHandleColor = UnityEditor.Handles.color;
                UnityEditor.Handles.color = viewColor;
                Vector3 startDir = Quaternion.AngleAxis(-halfAngle, up) * forward;
                UnityEditor.Handles.DrawSolidArc(origin, up, startDir, viewAngle, viewRadius);
                UnityEditor.Handles.color = prevHandleColor;
            }
            catch
            {
                // Ignore any editor-only drawing errors (defensive).
            }
#endif

            // Restore previous gizmo color
            Gizmos.color = prevGiz;
        }


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
