using UnityEngine;

public class SplineCarRNGColor : MonoBehaviour
{
    [SerializeField] Material[] mats;
    [SerializeField] MeshRenderer[] rends;
    private Material carMat;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carMat = mats[Random.Range(0, mats.Length)];
        
        foreach (MeshRenderer rend in rends)
        {
            rend.material = carMat;
        }
    }
}
