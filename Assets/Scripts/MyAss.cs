using UnityEngine;

public class MyAss : MonoBehaviour
{
    public float maxDistance = 0.1f;
    // Update is called once per frame
    void Update()
    {
        if(transform.localPosition.magnitude > maxDistance)
        {
            transform.localPosition = transform.localPosition.normalized * maxDistance;
        }
    }
}
