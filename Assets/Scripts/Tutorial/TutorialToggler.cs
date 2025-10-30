using Unity.Properties;
using UnityEngine;

public class TutorialToggler : MonoBehaviour
{

    public bool startTutorial = true;               
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetStartTutorial(bool val)
    {
               startTutorial = !startTutorial;
    }
  
}
