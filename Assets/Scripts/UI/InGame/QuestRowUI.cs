using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestRowUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image cardBackground;
    [SerializeField] private Outline cardOutline;
    [SerializeField] private Image categoryAccentBar;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private Image pinIcon;
    [SerializeField] private GameObject redNotificationDot;
    [SerializeField] private Button rowButton;

    [Header("Colors")]
    [SerializeField] private Color normalBgColor = new Color(0.12f, 0.15f, 0.20f, 0.88f);
    [SerializeField] private Color selectedBgColor = new Color(0.18f, 0.23f, 0.32f, 0.98f);
    [SerializeField] private Color normalTextColor = new Color(0.88f, 0.85f, 0.80f, 1f);
    [SerializeField] private Color selectedTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color accentSelectedColor = new Color(1.0f, 0.82f, 0.35f, 1f); // Warm Gold
    [SerializeField] private Color accentNormalColor = new Color(0.40f, 0.45f, 0.55f, 0.6f);

    private QuestItemData _boundData;
    private System.Action<QuestItemData> _onRowClicked;
    private bool _isSelected = false;

    public QuestItemData BoundData => _boundData;

    public void Bind(QuestItemData data, System.Action<QuestItemData> onClick = null)
    {
        _boundData = data;
        _onRowClicked = onClick;

        if (data == null)
            return;

        if (titleText != null)
        {
            titleText.text = data.questTitle;
        }

        if (subtitleText != null)
        {
            if (data.isLocked && !string.IsNullOrEmpty(data.lockRequirement))
            {
                subtitleText.gameObject.SetActive(true);
                subtitleText.text = data.lockRequirement;
            }
            else
            {
                subtitleText.gameObject.SetActive(false);
            }
        }

        if (distanceText != null)
        {
            bool hasDist = !string.IsNullOrEmpty(data.distanceText);
            distanceText.gameObject.SetActive(hasDist);
            if (hasDist) distanceText.text = data.distanceText;
        }

        if (pinIcon != null)
        {
            pinIcon.gameObject.SetActive(!string.IsNullOrEmpty(data.distanceText) || data.isTracking);
        }

        if (redNotificationDot != null)
        {
            redNotificationDot.SetActive(data.isTracking);
        }

        if (rowButton != null)
        {
            rowButton.onClick.RemoveListener(OnRowClicked);
            rowButton.onClick.AddListener(OnRowClicked);
        }

        SetSelected(false);
    }

    public void UpdateDistance(string dist)
    {
        if (distanceText != null)
        {
            bool hasDist = !string.IsNullOrEmpty(dist);
            distanceText.gameObject.SetActive(hasDist);
            if (hasDist) distanceText.text = dist;
        }

        if (pinIcon != null)
        {
            pinIcon.gameObject.SetActive(!string.IsNullOrEmpty(dist) || (_boundData != null && _boundData.isTracking));
        }
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;

        if (cardBackground != null)
        {
            cardBackground.color = selected ? selectedBgColor : normalBgColor;
        }

        if (cardOutline != null)
        {
            cardOutline.enabled = selected;
            cardOutline.effectColor = new Color(0.95f, 0.80f, 0.40f, 0.85f);
        }

        if (categoryAccentBar != null)
        {
            categoryAccentBar.gameObject.SetActive(true);
            categoryAccentBar.color = selected ? accentSelectedColor : accentNormalColor;
        }

        if (titleText != null)
        {
            titleText.color = selected ? selectedTextColor : normalTextColor;
        }
    }

    private void OnRowClicked()
    {
        if (_boundData != null)
        {
            _onRowClicked?.Invoke(_boundData);
        }
    }
}


