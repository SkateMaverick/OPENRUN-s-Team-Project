using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enums;

public class GameOptionsUI : MonoBehaviour
{
    [Header("Panel Root")]
    [SerializeField] private GameObject optionsPanel;

    [Header("Mouse Sensitivity")]
    [SerializeField] private UnityEngine.UI.Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;
    [SerializeField] private float minSensitivity = 0.1f;
    [SerializeField] private float maxSensitivity = 3.0f;

    [Header("Graphic Quality Buttons")]
    [SerializeField] private UnityEngine.UI.Button lowGraphicButton;
    [SerializeField] private UnityEngine.UI.Button middleGraphicButton;
    [SerializeField] private UnityEngine.UI.Button highGraphicButton;

    [Header("Quality Button Colors / Visuals")]
    [SerializeField] private Color selectedColor = new Color(1f, 0.85f, 0.4f, 1f); // Golden / highlighted
    [SerializeField] private Color unselectedColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);
    [SerializeField] private Color selectedTextColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color unselectedTextColor = new Color(0.8f, 0.8f, 0.8f, 0.7f);

    [Header("Close / Back Button")]
    [SerializeField] private UnityEngine.UI.Button closeButton;

    private const string UI_KEY = "GameOptionsUI";

    private void Awake()
    {
        if (optionsPanel == null)
        {
            optionsPanel = gameObject;
        }
    }

    private void Start()
    {
        // Setup Sensitivity Slider
        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = minSensitivity;
            sensitivitySlider.maxValue = maxSensitivity;
            sensitivitySlider.onValueChanged.RemoveListener(OnSensitivityChanged);
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }

        // Setup Graphic Quality Buttons
        if (lowGraphicButton != null)
        {
            lowGraphicButton.onClick.RemoveListener(OnLowGraphicClicked);
            lowGraphicButton.onClick.AddListener(OnLowGraphicClicked);
        }

        if (middleGraphicButton != null)
        {
            middleGraphicButton.onClick.RemoveListener(OnMiddleGraphicClicked);
            middleGraphicButton.onClick.AddListener(OnMiddleGraphicClicked);
        }

        if (highGraphicButton != null)
        {
            highGraphicButton.onClick.RemoveListener(OnHighGraphicClicked);
            highGraphicButton.onClick.AddListener(OnHighGraphicClicked);
        }

        // Setup Close Button
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Close);
            closeButton.onClick.AddListener(Close);
        }

        RefreshAllUI();
    }

    private void OnEnable()
    {
        GameSettingsManager.OnMouseSensitivityChanged += HandleSensitivityChangedFromManager;
        GameSettingsManager.OnGraphicQualityChanged += HandleGraphicQualityChangedFromManager;
        RefreshAllUI();
    }

    private void OnDisable()
    {
        GameSettingsManager.OnMouseSensitivityChanged -= HandleSensitivityChangedFromManager;
        GameSettingsManager.OnGraphicQualityChanged -= HandleGraphicQualityChangedFromManager;
    }

    public void Open()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }

        RefreshAllUI();

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.OpenUI(UI_KEY);
        }
    }

    public void Close()
    {
        if (optionsPanel != null && optionsPanel != gameObject)
        {
            optionsPanel.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }
    }

    public void Toggle()
    {
        bool isActive = (optionsPanel != null) ? optionsPanel.activeSelf : gameObject.activeSelf;
        if (isActive)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    private void OnSensitivityChanged(float value)
    {
        GameSettingsManager.MouseSensitivity = value;
        UpdateSensitivityText(value);
    }

    private void OnLowGraphicClicked()
    {
        GameSettingsManager.GraphicQuality = GraphicQualityLevel.Low;
        UpdateQualityButtonsVisual(GraphicQualityLevel.Low);
    }

    private void OnMiddleGraphicClicked()
    {
        GameSettingsManager.GraphicQuality = GraphicQualityLevel.Middle;
        UpdateQualityButtonsVisual(GraphicQualityLevel.Middle);
    }

    private void OnHighGraphicClicked()
    {
        GameSettingsManager.GraphicQuality = GraphicQualityLevel.High;
        UpdateQualityButtonsVisual(GraphicQualityLevel.High);
    }

    private void HandleSensitivityChangedFromManager(float newSens)
    {
        if (sensitivitySlider != null && !Mathf.Approximately(sensitivitySlider.value, newSens))
        {
            sensitivitySlider.SetValueWithoutNotify(newSens);
        }
        UpdateSensitivityText(newSens);
    }

    private void HandleGraphicQualityChangedFromManager(GraphicQualityLevel quality)
    {
        UpdateQualityButtonsVisual(quality);
    }

    private void RefreshAllUI()
    {
        float currentSens = GameSettingsManager.MouseSensitivity;
        if (sensitivitySlider != null)
        {
            sensitivitySlider.SetValueWithoutNotify(currentSens);
        }
        UpdateSensitivityText(currentSens);

        GraphicQualityLevel currentQuality = GameSettingsManager.GraphicQuality;
        UpdateQualityButtonsVisual(currentQuality);
    }

    private void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
        {
            sensitivityValueText.text = $"{value:0.00}x";
        }
    }

    private void UpdateQualityButtonsVisual(GraphicQualityLevel currentQuality)
    {
        SetButtonVisual(lowGraphicButton, currentQuality == GraphicQualityLevel.Low);
        SetButtonVisual(middleGraphicButton, currentQuality == GraphicQualityLevel.Middle);
        SetButtonVisual(highGraphicButton, currentQuality == GraphicQualityLevel.High);
    }

    private void SetButtonVisual(UnityEngine.UI.Button button, bool isSelected)
    {
        if (button == null) return;

        var img = button.GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.color = isSelected ? selectedColor : unselectedColor;
        }

        var text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = isSelected ? selectedTextColor : unselectedTextColor;
            text.fontStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;
        }
    }
}
