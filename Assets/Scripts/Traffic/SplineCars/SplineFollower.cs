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
        if (splineAnimate.IsPlaying == false && currentSpline != splines.Count - 1)
        {

            StartCoroutine(SwitchSpline());

        }
        AdjustCarSpeed();
    }


    IEnumerator SwitchSpline()
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
        splineAnimate.Restart(true);

    }

    private void AdjustCarSpeed()
    {
        float distance = Vector3.Distance(transform.position, nextSplineStartPoint);


        if (distance < 10f && !onLastSpline)
        {
            Debug.Log(transform.position);
            Debug.Log($"Distance to next spline start: {distance}");
            float t = 1f - (distance / 10f);
            Debug.Log($"Adjusting speed with t={t}");
            float adjustedSpeed = Mathf.Lerp(currentSplineInfo.TraversalSpeed, nextSplineSpeed, t);
            splineAnimate.MaxSpeed = adjustedSpeed;
        }
        else
        {
            splineAnimate.MaxSpeed = currentSplineInfo.TraversalSpeed;
        }
    }

}
