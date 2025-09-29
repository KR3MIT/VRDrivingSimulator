using UnityEngine;

public class NPCController : MonoBehaviour
{
    public bool reachedDestination;
    public float moveSpeed = 2f;
    public float stoppingDistance = 0.1f;

    [SerializeField]
    private Vector3? destination;

    void Update()
    {
        if (destination.HasValue)
        {
            Vector3 target = destination.Value;
            Vector3 direction = target - transform.position;
            direction.y = 0f; // Ignore vertical movement

            float distance = direction.magnitude;
            if (distance > stoppingDistance)
            {
                reachedDestination = false;
                Vector3 move = direction.normalized * moveSpeed * Time.deltaTime;
                if (move.magnitude > distance)
                    move = direction; // Don't overshoot

                transform.position += move;
                if (move != Vector3.zero)
                    transform.forward = move.normalized;
            }
            else
            {
                reachedDestination = true;
                destination = null;
            }
        }
    }

    public void SetDestination(Vector3 position)
    {
        destination = position;
        reachedDestination = false;
    }
}
