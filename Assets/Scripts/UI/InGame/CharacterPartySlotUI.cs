using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterPartySlotUI : MonoBehaviour
{
    [Header("Configuration")]
    public Enums.CharacterType characterType;
    public string characterName = "Character";
    public int slotNumber = 1;

    [Header("UI References")]
    public Image cardBackgroundImage;
    public Image avatarImage;
    public Image avatarFrameImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI numberText;
    public Image numberBadgeImage;
    public Slider healthBar;
    public Image activeIndicator;
    public Button slotButton;

    [Header("Visual Colors")]
    public Color normalCardColor = new Color(0.12f, 0.14f, 0.18f, 0.70f);
    public Color activeCardColor = new Color(0.20f, 0.28f, 0.38f, 0.95f);
    public Color activeGlowColor = new Color(1f, 1f, 1f, 1f);
    public Color inactiveGlowColor = new Color(0.8f, 0.8f, 0.8f, 0.4f);

    private bool _isActive = false;

    private void Awake()
    {
        if (slotButton == null)
        {
            slotButton = GetComponent<Button>();
        }

        if (slotButton != null)
        {
            slotButton.onClick.RemoveListener(OnSlotClicked);
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    private void Start()
    {
        ApplySlotData();
    }

    public void ApplySlotData()
    {
        if (nameText != null)
        {
            nameText.text = characterName;
        }

        if (numberText != null)
        {
            numberText.text = slotNumber.ToString();
        }
    }

    public void SetActiveState(bool isActive)
    {
        _isActive = isActive;

        if (cardBackgroundImage != null)
        {
            cardBackgroundImage.color = isActive ? activeCardColor : normalCardColor;
        }

        if (nameText != null)
        {
            nameText.color = isActive ? Color.white : new Color(0.82f, 0.85f, 0.9f, 0.85f);
            nameText.fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
        }

        if (activeIndicator != null)
        {
            activeIndicator.gameObject.SetActive(isActive);
        }

        if (avatarFrameImage != null)
        {
            avatarFrameImage.color = isActive ? activeGlowColor : inactiveGlowColor;
        }

        // Slight scale punch / offset for active slot
        var rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localScale = isActive ? new Vector3(1.05f, 1.05f, 1.05f) : Vector3.one;
        }
    }

    public void SetHealth(float current, float max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = current;
        }
    }

    private void OnSlotClicked()
    {
        if (PlayerController.Instance == null) return;

        if (characterType == Enums.CharacterType.BoxGolem)
        {
            PlayerController.Instance.SwitchToBoxGolem();
        }
        else if (characterType == Enums.CharacterType.SphereGolem)
        {
            PlayerController.Instance.SwitchToSphereGolem();
        }
    }
}
