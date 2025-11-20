using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorCard : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    public Image checkmark;
    public Image crossmark;

    public void Initialize(string title, string description, bool isFailed)
    {
        titleText.text = title;
        descriptionText.text = description;

        checkmark.gameObject.SetActive(!isFailed);
        crossmark.gameObject.SetActive(isFailed);
    }
}
