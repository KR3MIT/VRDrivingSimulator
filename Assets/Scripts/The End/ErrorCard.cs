using TMPro;
using UnityEngine;

public class ErrorCard : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text severityText;

    public void Initialize(string title, string description, string severity, Color color)
    {
        titleText.text = title;
        descriptionText.text = description;
        severityText.text = severity;

        severityText.color = color;
    }
}
