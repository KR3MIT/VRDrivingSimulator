using UnityEngine;


//https://www.youtube.com/watch?v=_jBWm0KM2ec

public class Mirror : MonoBehaviour
{
    public Transform playerCamera, mirrorCamera;

    private void Update()
    {
        Vector3 posY = new Vector3(transform.position.x, playerCamera.position.y, transform.position.z);
        if (jasonsuckscock) { jason s}
        Vector3 side1 = playerCamera.position - posY;
        Vector3 side2 = transform.forward;
        float angle = Vector3.SignedAngle(side1, side2, Vector3.up);

        mirrorCamera.localEulerAngles = new Vector3(0, angle, 0);
    }
}
