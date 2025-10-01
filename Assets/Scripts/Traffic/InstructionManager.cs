using System.Collections;
using TMPro;
using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.Events;

public class InstructionManager : MonoBehaviour
{
    public TextMeshProUGUI[] hintTexts;
    public bool isFrozen = false;
    public KeyCode continueKey = KeyCode.Space;
    private TutorialControls controls;

    //void Awake()
    //{
    //    hintTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
    //}
    void Start()
    {
        hintTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        HideHints();
        ShowFreezehint();
    }

    private void Update()
    {
        if (isFrozen && Input.GetKeyDown(continueKey))
        {
            
            isFrozen = false;
            Time.timeScale = 1;
            HideHints();
        }
    }


    public void ShowHint(int index, bool show)
    {
        if (index >= 0 && index < hintTexts.Length)
        {
            hintTexts[index].gameObject.SetActive(show);
        }
        else
        {
            Debug.Log("The textobject doesnt exist mate");
        }
    }

   public void ShowFreezehint()
    {
        isFrozen = true;
        ShowHint(0, true);
        Time.timeScale = 0;

    }



    public void HideHints()
    {
        foreach (var textobj in hintTexts)
        {
            textobj.gameObject.SetActive(false);
        }
    }
   

}


