using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{

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
        float distance = Vector3.Distance(transform.position, nextSplineStartPoint);
        Debug.Log(nextSplineStartPoint); 
        
        nextSplineLocked = nextSplineInfo.isSplineLocked;

        float distanceThreshold = currentSplineIsATurn ? speedBlendDisOnTurn : speedBlendDisOffTurn;

        if (distance < distanceThreshold && !onLastSpline && !nextSplineLocked)
        {
            float prevProgress = splineAnimate.NormalizedTime;
            float t = 1f - (distance / distanceThreshold);
            float adjustedSpeed = Mathf.Lerp(currentSplineInfo.TraversalSpeed, nextSplineSpeed, t);
            splineAnimate.MaxSpeed = adjustedSpeed;
            splineAnimate.NormalizedTime = prevProgress;
        }
        else if (nextSplineLocked)
        {
            float prevProgress = splineAnimate.NormalizedTime;
            float t = 1f - (distance / distanceThreshold);
            float adjustedSpeed = Mathf.Lerp(currentSplineInfo.TraversalSpeed, 0, t);
            splineAnimate.MaxSpeed = adjustedSpeed;
            splineAnimate.NormalizedTime = prevProgress;
        }
        else
        {
            splineAnimate.MaxSpeed = currentSplineInfo.TraversalSpeed;
        }
    }

}
