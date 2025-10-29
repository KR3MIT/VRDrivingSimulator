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
    public LogitechInput LogitechInput;
    public TutorialTextScriptableObject tutorialText;
    public List<Transform> spawnPoints;

    private void Start()
    {
        InstructionManager = FindFirstObjectByType<InstructionManager>();
        LogitechInput = FindFirstObjectByType<LogitechInput>();
    }
    public void OnTriggerEnter(Collider collision)
    {
        //Left Turn Task
        if (collision.gameObject == TutorialColliders[0])
        {
           // Debug.Log("Hit Tutorial1");
            InstructionManager.ShowFreezeHint(0, true, tutorialText.LeftTurnTask);
            StartCoroutine(LeftOrientationCheck());
        }
        //Left Blinker Task
        if (collision.gameObject == TutorialColliders[1])
        {
            //Debug.Log("Hit Tutorial2");
            InstructionManager.ShowFreezeHint(0, true, tutorialText.LeftBlinkerTask);
            //StartCoroutine(LeftBlinkCheck());
        }
    }
   
    IEnumerator LeftOrientationCheck()
    {
        bool OpsDirection = true;
        bool PedDirection = true;
        InstructionManager.ShowHint(1, true, tutorialText.LeftTurnStep1);
        Arrow = Instantiate(Arrow, spawnPoints[0].position, spawnPoints[0].rotation);
        Arrow.SetActive(true);

        while (OpsDirection)
        {
            //Debug.DrawLine(PlayerCamera.transform.position, PlayerCamera.transform.position + PlayerCamera.transform.forward * 100f, Color.red, 1f);
            if (Physics.BoxCast(PlayerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), PlayerCamera.transform.forward, out RaycastHit hitInfo, PlayerCamera.transform.rotation, 100f))
            { 
                if(hitInfo.collider.CompareTag("Arrow"))
                {
                    Debug.Log("Hit1");
                  
                    OpsDirection = false;
                }
                else
                {
                    Debug.Log("no hit1");
                }

            }
                yield return new WaitForSeconds(CheckDelay);
        }

        InstructionManager.ShowHint(1, true, tutorialText.LeftTurnStep2);
        Arrow.transform.position = spawnPoints[1].position;
        Arrow.transform.rotation = spawnPoints[1].rotation;
        while (PedDirection)
        {
            //Debug.DrawLine(PlayerCamera.transform.position, PlayerCamera.transform.position + PlayerCamera.transform.forward * 100f, Color.red, 1f);
            if (Physics.BoxCast(PlayerCamera.transform.position, new Vector3(0.5f, 0.5f, 0.5f), PlayerCamera.transform.forward, out RaycastHit hitInfo, PlayerCamera.transform.rotation, 100f))
            {
                if (hitInfo.collider.CompareTag("Arrow"))
                {
                    Debug.Log("Hit2");
                    Arrow.gameObject.SetActive(false);
                    PedDirection = false;
                }
                else
                {
                    Debug.Log("No Hit2");
                }
            }
            yield return new WaitForSeconds(CheckDelay);
        }
        InstructionManager.ShowHint(1, true, tutorialText.LeftTurnDone);
        InstructionManager.allowContinue = true;
    }

    //IEnumerator LeftBlinkCheck()
    //{
    //    bool Blinked = false;
    //    InstructionManager.ShowHint(1, true, TutorialText.LeftBlinkerStep1);
    //    while (!Blinked)
    //    {
    //        if (LogitechInput.leftBlinker == true)
    //        {
    //            Blinked = true;
    //        }
    //        yield return new WaitForSeconds(CheckDelay);
       
    //    }
    //    InstructionManager.allowContinue = true;
    //}


    //IEnumerator RightOrientationCheck()
    //{
    //    
    //}
}
