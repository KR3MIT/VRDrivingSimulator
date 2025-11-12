using UnityEngine;


public class S2CarSpawner : MonoBehaviour
{
    public SplineCarSpawner splineCarSpawner;
    public int spawnChance;
    private static int index = 0;
    private int maxIndex = 10;
    private int chanceIndex = 2;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spawnChance = Random.Range(index, maxIndex);
            if (spawnChance >= chanceIndex)
            {

                Debug.Log("Succes in spawning, chance" + spawnChance);
                splineCarSpawner.TriggerCarSpawn();
                index = 0;
            }
            else
            {
                splineCarSpawner.routeIndex++;
                index++;
                Debug.Log("FAILED in spawning, chance" + spawnChance);

            }

        }
    }

}
