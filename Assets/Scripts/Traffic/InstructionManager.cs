using System;
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
    public bool allowContinue = false;

    void Start()
    {
        input = GetComponent<PlayerInput>();
        continueTime = input.actions["Continue"];
        continueTime.Enable();
        hintTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        HideHints();
        ShowFreezehint(0,true,"suck my ass");
        ShowHint(1,true,"Press 'Space' to resume");
    }
    private void Update()
    {

        Debug.Log(Time.timeScale);
        if (isFrozen && continueTime.triggered && allowContinue)
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
    public void ShowFreezehint(int index, bool show, string text)
    {
        isFrozen = true;
        if (index >= 0 && index < hintTexts.Length)
        {
            hintTexts[index].gameObject.SetActive(show);
            hintTexts[index].text = text;
        }
        else
        {
            Debug.Log("The textobject doesnt exist mate");
        }
        StartCoroutine(SmoothStop());
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
        
        while (Time.timeScale < realTime && allowContinue)
        {
            Time.timeScale += Time.unscaledDeltaTime / resumeDuration * realTime;
            yield return null;
        }
        
        Time.timeScale = realTime;
        allowContinue = false;

    }
    private IEnumerator SmoothStop()
    {

        Debug.Log("Freezing game");

        while (Time.timeScale > 0.1f && !allowContinue)
        {
            Time.timeScale -= Time.unscaledDeltaTime / resumeDuration * realTime;
            yield return null;
        }
      
        Time.timeScale = 0;
        allowContinue = true;

    }
}


