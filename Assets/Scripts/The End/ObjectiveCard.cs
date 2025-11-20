using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveCard", menuName = "Scriptable Objects/ObjectiveCard")]
public class ObjectiveCard : ScriptableObject
{
    public string titleText;
    public string descriptionGoodText;
    public string descriptionBadText;
    public ObjectiveType objectiveType;
    public enum ObjectiveType
    {
        SpeedLimit,
        RightOfWay,
        TrafficLight,
        LaneViolation,
        Stopline,
        PedestrianHit,
        CarHit,
        PavementHit,
        Blinkers,
        MirrorCkeck,
        Orientation,


    }
}
