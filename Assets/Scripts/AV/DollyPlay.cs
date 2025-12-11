using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class DollyPlay : MonoBehaviour
{
    private CinemachineSplineDolly dolly;
    public float speed = 5f;
    public PlayerInput input;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        dolly = GetComponent<CinemachineSplineDolly>();
        input.actions["Test"].performed += ctx => StartCoroutine(PlayDolly());
    }

    private IEnumerator PlayDolly()
    {
        float t = 0f;
        var dollyLength = dolly.Spline.CalculateLength();
        while (t < 1f)
        {
            t += Time.deltaTime * speed / dollyLength;
            dolly.CameraPosition = Mathf.Clamp01(t);
            yield return null;
        }
    }
}
