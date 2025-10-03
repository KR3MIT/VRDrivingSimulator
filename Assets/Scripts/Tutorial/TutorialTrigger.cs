using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{

    public InstructionManager InstructionManager;
    public List<GameObject> TutorialColliders;
    public float CheckDelay = 1f;
    public GameObject Arrow;
    public Camera PlayerCamera;

    public void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject == TutorialColliders[0])
        {
            Debug.Log("Hit Collider");
            InstructionManager.ShowFreezeHint(0, true, "Opgave: Orienter dig rigtigt, før du udfører dit venstresving");
           // InstructionManager.allowContinue = true;
            StartCoroutine(LeftOrientationCheck());
        }
    }

    IEnumerator LeftOrientationCheck()
    {
        bool OpsDirection = true;
        bool PedDirection = true;
        InstructionManager.ShowHint(1, true, "Kig efter modkørende biler");
        Arrow = Instantiate(Arrow, new Vector3(5, 5, 10), Quaternion.Euler(0, 90, 0));
       
        while (OpsDirection)
        {
            //Debug.DrawLine(PlayerCamera.transform.position, PlayerCamera.transform.position + PlayerCamera.transform.forward * 100f, Color.red, 1f);
            if (Physics.BoxCast(PlayerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), PlayerCamera.transform.forward, out RaycastHit hitInfo, PlayerCamera.transform.rotation, 100f))
            { 
                if(hitInfo.collider.CompareTag("Arrow"))
                {
                    Debug.Log("Hit1");
                    //DestroyImmediate(Arrow, true);
                    OpsDirection = false;
                }
                else
                {
                    Debug.Log("no hit1");
                }

            }
                yield return new WaitForSeconds(CheckDelay);
        }

        InstructionManager.ShowHint(1, true, "Kig efter forbipasserende mennesker og cykelister ved fodgængerovergang");
        Arrow.transform.position = new Vector3(-5, 5, 10);
        while (PedDirection)
        {
            //Debug.DrawLine(PlayerCamera.transform.position, PlayerCamera.transform.position + PlayerCamera.transform.forward * 100f, Color.red, 1f);
            if (Physics.BoxCast(PlayerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), PlayerCamera.transform.forward, out RaycastHit hitInfo, PlayerCamera.transform.rotation, 100f))
            {
                if (hitInfo.collider.CompareTag("Arrow"))
                {
                    Debug.Log("Hit2");
                    DestroyImmediate(Arrow, true);
                    PedDirection = false;
                }
                else
                {
                    Debug.Log("No Hit2");
                }
            }
            yield return new WaitForSeconds(CheckDelay);
        }
        InstructionManager.ShowHint(1, true, "Godt klaret! Tryk på *INSERT BUTTON* for at fortsætte.");
        InstructionManager.allowContinue = true;
    }

    // lav flere IEnumerator til de andre opgaver og giv dem en collider
}
