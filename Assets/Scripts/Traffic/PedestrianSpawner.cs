using System.Collections;
using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    public GameObject pedestrianPrefab;
    public int spawnCount = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnPedestrians());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnPedestrians()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject obj = Instantiate(pedestrianPrefab);
            Transform child = transform.GetChild(Random.Range(0, transform.childCount - 1));
            obj.GetComponent<WaypointNavigation>().currentWaypoint = child.GetComponent<Waypoint>();
            obj.transform.position = child.position;
            yield return new WaitForEndOfFrame();
        }
    }
}
