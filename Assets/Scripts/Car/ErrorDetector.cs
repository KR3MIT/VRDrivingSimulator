using UnityEngine;
using UnityEngine.Splines;

public class ErrorDetector : MonoBehaviour
{
    public SplineContainer spline;  // Assign in Inspector
    public Transform car;
    public float allowedDistance = 2f;
    public Vector3 nearestPoint;
    void Update()
    {
       

        Vector3 carPos = car.position;
        Spline splineRef = spline.Spline;
        var ray = new Ray(carPos + Vector3.up * 50f, Vector3.down);
        SplineUtility.GetNearestPoint(splineRef, ray, out carPos, out float nearestT, out Vector3 nearestPoint);

        float distance = Vector3.Distance(carPos, nearestPoint);

        if (distance > allowedDistance)
        {
            Debug.Log("Car is off the spline!");
        }
    }
}
