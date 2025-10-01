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
    SplineInfo nextSplineInfo;
    float nextSplineSpeed;
    Vector3 nextSplineStartPoint;
    bool onLastSpline = false;
    bool nextSplineOpen;



    SplineAnimate splineAnimate;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
    }

    // Update is called once per frame
    void Update()
    {
        if (splineAnimate.IsPlaying == false)
        {

            //StartCoroutine(SwitchSpline());
            SwitchSpline();
        }
        AdjustCarSpeed();
        //Debug.Log(transform.position);
    }

    void SwitchSpline()
    {
        if (onLastSpline) {
            Destroy(gameObject);
            return;
        }
        //splineAnimate.Pause();
        currentSpline++;
        splineAnimate.Container = splines[currentSpline];
        currentSplineInfo = splines[currentSpline].GetComponent<SplineInfo>();

        if (currentSpline < splines.Count - 1)
        {
            nextSplineInfo = splines[currentSpline + 1].GetComponent<SplineInfo>();
            nextSplineSpeed = nextSplineInfo.TraversalSpeed;
            nextSplineStartPoint = nextSplineInfo.SplineStartPoint;
        }
        else
        {
            onLastSpline = true;
        }
        AdjustCarSpeed();
        splineAnimate.Restart(true);
    }

    /*IEnumerator SwitchSpline()
    {
        if (onLastSpline) Destroy(gameObject);

        currentSpline++;
        splineAnimate.Container = splines[currentSpline];
        currentSplineInfo = splines[currentSpline].GetComponent<SplineInfo>();
        
        if (currentSpline < splines.Count - 1)
        {
            nextSplineInfo = splines[currentSpline + 1].GetComponent<SplineInfo>();
            nextSplineSpeed = nextSplineInfo.TraversalSpeed;
            nextSplineStartPoint = nextSplineInfo.SplineStartPoint;
        }
        else
        {
            onLastSpline = true;
        }
        AdjustCarSpeed();
       

        splineAnimate.Play();
        yield return new WaitForSeconds(0.01f);
        splineAnimate.Pause();
        splineAnimate.Restart(false);
        splineAnimate.Play();

    }*/

    private void AdjustCarSpeed()
    {
        float distance = Vector3.Distance(transform.position, nextSplineStartPoint);
        Debug.Log(nextSplineStartPoint);
        //Debug.Log($"Distance to next spline start: {distance}");
        //Debug.Log($"Current spline speed: {currentSplineInfo.TraversalSpeed}, Next spline speed: {nextSplineSpeed}");

        if (distance < 10f && !onLastSpline)
        {
            float prevProgress = splineAnimate.NormalizedTime;
            float t = 1f - (distance / 10f);
            float adjustedSpeed = Mathf.Lerp(currentSplineInfo.TraversalSpeed, nextSplineSpeed, t);splineAnimate.MaxSpeed = adjustedSpeed;
            splineAnimate.MaxSpeed = adjustedSpeed;
            splineAnimate.NormalizedTime = prevProgress;
        }
        else
        {
            splineAnimate.MaxSpeed = currentSplineInfo.TraversalSpeed;
        }
    }

}
