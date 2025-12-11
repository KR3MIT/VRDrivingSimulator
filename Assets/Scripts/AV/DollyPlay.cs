using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class DollyPlay : MonoBehaviour
{
    private CinemachineSplineDolly dolly;
    public float speed;
    public bool dampening;
    public float damp;
    public PlayerInput input;
    private float velocity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        dolly = GetComponent<CinemachineSplineDolly>();
        input.actions["Test"].performed += ctx => StartCoroutine(PlayDolly());
    }

    private IEnumerator PlayDolly()
    {
        dolly.CameraPosition = 0f;
        float t = 0f;
        float targetT = 1f;
        velocity = 0f;
        var dollyLength = dolly.Spline.CalculateLength();
        
        while (t < 1)
        {
            if (!dampening) 
            { 
                t += Time.deltaTime * speed / dollyLength;
                dolly.CameraPosition = Mathf.Clamp01(t);
            }
            else
            {
                t = Mathf.SmoothDamp(t, targetT, ref velocity, damp, speed / dollyLength);
                dolly.CameraPosition = Mathf.Clamp01(t);
            }
            yield return null;
        }
        
        // Ensure we reach the end
        dolly.CameraPosition = 1f;
    }
}
