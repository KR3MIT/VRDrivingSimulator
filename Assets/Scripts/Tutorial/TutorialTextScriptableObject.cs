using UnityEngine;

[CreateAssetMenu(fileName = "TutorialText", menuName = "ScriptableObjects/TutorialTextScriptableObject", order = 1)]
public class TutorialTextScriptableObject : ScriptableObject
{
    public string LeftTurnTask;
    public string LeftTurnStep1;
    public string LeftTurnStep2;
    public string LeftTurnDone;
    public string LeftBlinkerTask;
    public string LeftBlinkerStep1;
    public string ErrorPedestrian;
    public string ErrorCarCollision;
    public string ErrorRoad;

}
