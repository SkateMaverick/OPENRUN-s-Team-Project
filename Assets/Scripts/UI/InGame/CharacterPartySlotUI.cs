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
    public Image activeArrowImage;
    public Image burstGemImage;
    public Button slotButton;

    [Header("Visual Colors")]
    public Color normalCardColor = new Color(0f, 0f, 0f, 0f); // Floating transparent
    public Color activeCardColor = new Color(0.15f, 0.2f, 0.28f, 0.35f); // Subtle soft highlight
    public Color activeGlowColor = new Color(1f, 1f, 1f, 1f);
    public Color inactiveGlowColor = new Color(0.75f, 0.78f, 0.85f, 0.65f);

    private bool _isActive = false;
    private PlayerHealth _boundHealth;

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
        BindHealth();
    }

    private void OnDestroy()
    {
        if (_boundHealth != null)
        {
            _boundHealth.OnHealthChanged -= OnHealthChanged;
        }
    }

    public void BindHealth()
    {
        if (_boundHealth != null)
        {
            _boundHealth.OnHealthChanged -= OnHealthChanged;
            _boundHealth = null;
        }

        var pc = PlayerController.Instance != null ? PlayerController.Instance : Object.FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            GameObject charObj = (characterType == Enums.CharacterType.BoxGolem) ? pc.boxGolem : pc.sphereGolem;
            if (charObj != null)
            {
                _boundHealth = charObj.GetComponent<PlayerHealth>();
                if (_boundHealth != null)
                {
                    _boundHealth.OnHealthChanged += OnHealthChanged;
                    SetHealth(_boundHealth.Health, _boundHealth.StartingHealth);
                }
            }
        }
    }

    private void OnHealthChanged(float current, float max)
    {
        SetHealth(current, max);
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
            nameText.color = isActive ? Color.white : new Color(0.9f, 0.92f, 0.95f, 0.9f);
            nameText.fontStyle = isActive ? FontStyles.Bold : FontStyles.Normal;
        }

        // Active character shows left arrow, teammate shows burst gem
        if (activeArrowImage != null)
        {
            activeArrowImage.gameObject.SetActive(isActive);
        }

        if (burstGemImage != null)
        {
            burstGemImage.gameObject.SetActive(!isActive);
        }

        // Teammates show their slim HP bar; active character HP is displayed at bottom center
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(!isActive);
        }

        if (avatarFrameImage != null)
        {
            avatarFrameImage.color = isActive ? activeGlowColor : inactiveGlowColor;
        }

        if (numberBadgeImage != null)
        {
            numberBadgeImage.color = isActive ? Color.white : new Color(0.85f, 0.88f, 0.92f, 0.85f);
        }

        // Subtle punch for active slot
        var rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localScale = isActive ? new Vector3(1.04f, 1.04f, 1.04f) : Vector3.one;
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
        var pc = PlayerController.Instance != null ? PlayerController.Instance : Object.FindFirstObjectByType<PlayerController>();
        if (pc == null) return;

        if (characterType == Enums.CharacterType.BoxGolem)
        {
            pc.SwitchToBoxGolem();
        }
        else if (characterType == Enums.CharacterType.SphereGolem)
        {
            pc.SwitchToSphereGolem();
        }
    }
}
