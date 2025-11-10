using UnityEngine;

public class InfiniteRoad : MonoBehaviour
{
    public GameObject roadSegment;
    public int extraSegments = 3;
    public float segmentLength = 80f;

    private Transform playerTransform;

    private int segmentsPositive = 0;
    private int segmentsNegative = 0;
    private float defaultZ;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        defaultZ = transform.position.z;

        for (int i = 1; i <= extraSegments; i++)
        {
            Instantiate(roadSegment, new Vector3(transform.position.x, transform.position.y, defaultZ + segmentLength * i), Quaternion.Euler(0, 90, 0));
            Instantiate(roadSegment, new Vector3(transform.position.x, transform.position.y, defaultZ - segmentLength * i), Quaternion.Euler(0, 90, 0));
            segmentsPositive++;
            segmentsNegative++;
        }
    }

    private void Update()
    {
        float playerSegmentsPositive = playerTransform.position.z + extraSegments * segmentLength;
        float furthestPositiveZ = defaultZ + segmentsPositive * segmentLength;
        while (furthestPositiveZ < playerSegmentsPositive)
        {
            furthestPositiveZ += segmentLength;
            Instantiate(roadSegment, new Vector3(transform.position.x, transform.position.y, furthestPositiveZ), Quaternion.Euler(0, 90, 0));
            segmentsPositive++;
        }

        float playerSegmentsNegative = playerTransform.position.z - extraSegments * segmentLength;
        float furthestNegativeZ = defaultZ - segmentsNegative * segmentLength;
        while (furthestNegativeZ > playerSegmentsNegative)
        {
            furthestNegativeZ -= segmentLength;
            Instantiate(roadSegment, new Vector3(transform.position.x, transform.position.y, furthestNegativeZ), Quaternion.Euler(0, 90, 0));
            segmentsNegative++;
        }
    }
}
