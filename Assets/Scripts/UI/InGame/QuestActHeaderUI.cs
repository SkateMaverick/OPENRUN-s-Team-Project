using UnityEngine;
using TMPro;

public class QuestActHeaderUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;

    public void Setup(string title, string subtitle)
    {
        if (titleText != null)
        {
            titleText.text = title;
            var rt = titleText.rectTransform;
            if (string.IsNullOrEmpty(subtitle))
            {
                rt.anchorMin = new Vector2(0f, 0f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(-32f, 0f);
                titleText.alignment = TextAlignmentOptions.MidlineLeft;
            }
            else
            {
                rt.anchorMin = new Vector2(0f, 0.5f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.anchoredPosition = new Vector2(0f, -2f);
                rt.sizeDelta = new Vector2(-32f, -4f);
            }
        }

        if (subtitleText != null)
        {
            subtitleText.text = subtitle;
            subtitleText.gameObject.SetActive(!string.IsNullOrEmpty(subtitle));
        }
    }
}
