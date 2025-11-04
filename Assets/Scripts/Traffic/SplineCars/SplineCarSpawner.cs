using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineCarSpawner : MonoBehaviour
{

    public List<SplineRoute> splineRoutes;
    public List<GameObject> carPrefabs;
    [SerializeField] private float spawnIntervalMin = 2f;
    [SerializeField] private float spawnIntervalMax = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnCarRoutine());
    }

    IEnumerator SpawnCarRoutine()
    {
        while (true)
        {
            
            int routeIndex = Random.Range(0, splineRoutes.Count);
            int carIndex = Random.Range(0, carPrefabs.Count);

            SplineRoute selectedRoute = splineRoutes[routeIndex];
            GameObject carInstance = Instantiate(carPrefabs[carIndex], selectedRoute.transform);
            carInstance.transform.localPosition = carInstance.transform.localPosition + Vector3.down * 30f;//all routes are in 0,0,0 so move them so they dont appear for a frame
            SplineFollower carSplineFollower = carInstance.GetComponent<SplineFollower>();
            
            if (carSplineFollower == null)
            {
                Debug.LogError("The car prefab does not have a SplineFollower component.");
                Destroy(carInstance);
                yield return null;
            }
            
            carSplineFollower.splines = selectedRoute.splines;

            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));
        }
    }
}
