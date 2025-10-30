using UnityEngine;

public class EndZone : MonoBehaviour
{
    public enum ZoneType
    {
        FailEnd,
        FinishEnd,
    }

    public ZoneType zoneType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (zoneType)
            {
                case ZoneType.FailEnd:
                    GameEnder.Instance.EndGame(GameEnder.GameEndCondition.FailZone);
                    break;
                case ZoneType.FinishEnd:
                    GameEnder.Instance.EndGame(GameEnder.GameEndCondition.FinishZone);
                    break;
            }
        }
    }
}
