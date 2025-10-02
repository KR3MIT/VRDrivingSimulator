using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{

    public InstructionManager InstructionManager;
    public List<GameObject> TutorialColliders;
    public float CheckDelay = 2f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject == TutorialColliders[0])
        {
            Debug.Log("Hit Collider");
            InstructionManager.ShowFreezeHint(0, true, "Orienter dig rigtigt, før du udfører dit venstresving");
           // InstructionManager.allowContinue = true;
            LeftTurn();
        }
    }

    void LeftTurn()
    {
        //he look at the right thing
        StartCoroutine(LeftOrientationCheck());
        //InstructionManager.allowContinue = true;
    }

    IEnumerator LeftOrientationCheck()
    {
        bool OpsDirection = true;
        bool PedDirection = true;
        while (OpsDirection)
        {
            InstructionManager.ShowHint(1,true, "Kig efter modkørende biler");
            //spawn prefab of red arrow above opposite lane
            //logic to check if player is looking at the right direction
            yield return new WaitForSeconds(CheckDelay);

            //check condition
            OpsDirection = false;
        }
        while (PedDirection)
        {
            InstructionManager.ShowHint(1, true, "Kig efter forbipasserende mennesker og cykelister ved fodgængerovergang");
            //spawn prefab of red arrow above "overgangen" 
            //logic to check if player is looking the right direction
            yield return new WaitForSeconds(CheckDelay);

            //check condition
            PedDirection = false;
        }

        InstructionManager.allowContinue = true;
    }
}
