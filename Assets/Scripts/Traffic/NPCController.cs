using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    public List<GameObject> PedestrianPrefabs;

    public bool reachedDestination;
    public float moveSpeed = 2f;
    public float speedVariance = 0.2f;
    public float stoppingDistance = 0.1f;
    public float turnSpeed = 180f; // Degrees per second

    [SerializeField]
    private Vector3? destination;

    private void Start()
    {
        var randomPedestrianIndex = Random.Range(0, PedestrianPrefabs.Count);
        var pedestrian = Instantiate(PedestrianPrefabs[randomPedestrianIndex], transform);
        pedestrian.transform.localPosition = Vector3.zero;
        animator = pedestrian.GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();

        moveSpeed = Random.Range(moveSpeed - speedVariance, moveSpeed + speedVariance);
        agent.speed = moveSpeed;
    }

    void Update()
    {
        NavMeshMove();
    }

    private void NavMeshMove()
    {
        if (destination.HasValue)
        {
            Vector3 target = destination.Value;
            Vector3 direction = target - transform.position;
            direction.y = 0f; // Ignore vertical movement

            float distance = direction.magnitude;
            if (distance > stoppingDistance)
            {
                agent.SetDestination(target);
            }
            else
            {
                reachedDestination = true;
                destination = null;
            }
        }

        var normalizedSpeed = agent.velocity.magnitude / agent.speed;
        if(normalizedSpeed < .1f) {normalizedSpeed = 0f; }
        animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
    }

    private void OldMove()
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

                // Smoothly rotate towards movement direction
                if (move != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(move.normalized, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        turnSpeed * Time.deltaTime
                    );
                }
            }
            else
            {
                reachedDestination = true;
                destination = null;
            }
        }
    }

    public void RotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        //Debug.Log("Rotating towards " + target);
    }

    public void SetDestination(Vector3 position)
    {
        destination = position;
        reachedDestination = false;
    }
}
