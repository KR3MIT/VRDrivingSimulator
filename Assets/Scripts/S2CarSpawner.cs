using UnityEngine;


public class S2CarSpawner : MonoBehaviour
{
    public SplineCarSpawner splineCarSpawner;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            splineCarSpawner.TriggerCarSpawn();
        }
    }

}
