using UnityEngine;
using UnityEngine.Splines;

public class SplineInfo : MonoBehaviour
{

    [SerializeField] private float traversalSpeed;
    [SerializeField] private bool isATurn;
    private Vector3 splineStartPoint;
    private SplineContainer splineContainer;


    // Expose read-only properties for SplineFollower to access
    public float TraversalSpeed => traversalSpeed;
    public bool IsATurn => isATurn;
    public Vector3 SplineStartPoint => splineStartPoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        splineContainer = GetComponent<SplineContainer>();

        traversalSpeed = traversalSpeed / 3.6f; // Convert from km/h to m/s

        splineStartPoint = splineContainer.Spline.EvaluatePosition(0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
