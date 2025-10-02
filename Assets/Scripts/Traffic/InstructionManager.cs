using System.Collections;
using TMPro;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InstructionManager : MonoBehaviour
{
    public TextMeshProUGUI[] hintTexts;
    public bool isFrozen = false;
    private PlayerInput input;
    private InputAction continueTime;
    float resumeDuration = 2f;
    float realTime = 1f;


    void Start()
    {
        input = GetComponent<PlayerInput>();
        continueTime = input.actions["Continue"];
        continueTime.Enable();
        hintTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        HideHints();
        ShowFreezehint();
    }

    private void Update()
    {

        Debug.Log(Time.timeScale);
        if (isFrozen && continueTime.triggered)
        {
            
            isFrozen = false;
            StartCoroutine(SmoothResume());
            HideHints();
        }
    }


    public void ShowHint(int index, bool show, string text)
    {
        if (index >= 0 && index < hintTexts.Length)
        {
            hintTexts[index].gameObject.SetActive(show);
            hintTexts[index].text = text;
        }
        else
        {
            Debug.Log("The textobject doesnt exist mate");
        }
    }

   public void ShowFreezehint()
    {
        isFrozen = true;
        ShowHint(0, true, "meget vigtig tutorial");
        ShowHint(1, true, "Tryk 'Mellemrum' for at fortsætte");
        Time.timeScale = 0;
    }



    public void HideHints()
    {
        foreach (var textobj in hintTexts)
        {
            textobj.gameObject.SetActive(false);
        }
    }
    private IEnumerator SmoothResume()
    {
        
        Debug.Log("Resuming game");
        
        while (Time.timeScale < realTime)
        {
            Time.timeScale += Time.unscaledDeltaTime / resumeDuration * realTime;
            yield return null;
        }
        Time.timeScale = realTime;


    }

}


