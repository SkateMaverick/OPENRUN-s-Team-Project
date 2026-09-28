using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBottomHealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI levelText;

    [Header("Settings")]
    public string defaultLevel = "Lv. 50";
    public bool showCurrentMaxText = true;

    private PlayerHealth _activePlayerHealth;

    private void Start()
    {
        if (levelText != null && string.IsNullOrEmpty(levelText.text))
        {
            levelText.text = defaultLevel;
        }

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged += OnCharacterChanged;
            BindToActiveCharacter(PlayerController.Instance.CurrentCharacterTransform);
        }
        else
        {
            FindAndBindAnyPlayerHealth();
        }
    }

    private void OnEnable()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
            PlayerController.Instance.OnCharacterChanged += OnCharacterChanged;
            BindToActiveCharacter(PlayerController.Instance.CurrentCharacterTransform);
        }
    }

    private void OnDisable()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
        }

        UnbindCurrentHealth();
    }

    private void OnDestroy()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
        }

        UnbindCurrentHealth();
    }

    private void OnCharacterChanged(Transform currentTransform)
    {
        BindToActiveCharacter(currentTransform);
    }

    public void BindToActiveCharacter(Transform characterTransform)
    {
        UnbindCurrentHealth();

        if (characterTransform != null)
        {
            _activePlayerHealth = characterTransform.GetComponent<PlayerHealth>();
        }

        if (_activePlayerHealth == null)
        {
            FindAndBindAnyPlayerHealth();
            return;
        }

        _activePlayerHealth.OnHealthChanged += UpdateHealthDisplay;
        UpdateHealthDisplay(_activePlayerHealth.Health, _activePlayerHealth.StartingHealth);
    }

    private void FindAndBindAnyPlayerHealth()
    {
        _activePlayerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        if (_activePlayerHealth != null)
        {
            _activePlayerHealth.OnHealthChanged += UpdateHealthDisplay;
            UpdateHealthDisplay(_activePlayerHealth.Health, _activePlayerHealth.StartingHealth);
        }
        else
        {
            // Default fallback
            UpdateHealthDisplay(100f, 100f);
        }
    }

    private void UnbindCurrentHealth()
    {
        if (_activePlayerHealth != null)
        {
            _activePlayerHealth.OnHealthChanged -= UpdateHealthDisplay;
            _activePlayerHealth = null;
        }
    }

    public void UpdateHealthDisplay(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = max;
            healthSlider.value = Mathf.Clamp(current, 0f, max);
        }

        if (hpText != null)
        {
            if (showCurrentMaxText)
            {
                hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
            }
            else
            {
                hpText.text = $"{Mathf.CeilToInt(current)}";
            }
        }
    }
}
