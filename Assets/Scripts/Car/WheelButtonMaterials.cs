using UnityEngine;

public class WheelButtonMaterials : MonoBehaviour
{
    public SkinnedMeshRenderer leftBumper, rightBumper, xboxButton;

    public Material activeMaterial, inactiveMaterial;

    public void SetActiveButton(int button)
    {
        switch (button)
        {
            case 0:
                leftBumper.material = activeMaterial;
                rightBumper.material = inactiveMaterial;
                xboxButton.material = inactiveMaterial;
                break;

            case 1:
                xboxButton.material = activeMaterial;
                leftBumper.material = inactiveMaterial;
                rightBumper.material = inactiveMaterial;
                break;

            case 2:
                rightBumper.material = activeMaterial;
                leftBumper.material = inactiveMaterial;
                xboxButton.material = inactiveMaterial;
                break;

            default:
                leftBumper.material = inactiveMaterial;
                rightBumper.material = inactiveMaterial;
                xboxButton.material = inactiveMaterial;
                break;
        }
    }
}
