using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineFollower : MonoBehaviour
{

    [SerializeField] private List<SplineContainer> splines;
    int currentSpline = 0;
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
    }


    IEnumerator SwitchSpline()
    {
        currentSpline++;
        splineAnimate.Container = splines[currentSpline];
        splineAnimate.Play();
        yield return new WaitForSeconds(0.01f);
        //splineAnimate.Pause();
        //splineAnimate.Restart(false);

    }


}
