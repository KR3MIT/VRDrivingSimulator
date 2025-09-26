using UnityEngine;

public class Waypoint : MonoBehaviour
{
   public Waypoint nextWaypoint;
   public Waypoint previousWaypoint;

    [Range (0f, 5f)]
    public float width = 0f;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * width / 2f;
        Vector3 maxBound = transform.position - transform.right * width / 2f;

        return Vector3.Lerp(minBound,maxBound,Random.Range(0f,1f));
    }

}
