using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class QuickPotionSlotUI : MonoBehaviour
{
    [Header("Potion Settings")]
    [SerializeField] private PotionItemData activePotion;
    [SerializeField] private KeyCode hotkey = KeyCode.Z;
    [SerializeField] private bool grantStarterPotionsIfEmpty = true;
    [SerializeField] private int starterPotionAmount = 10;

    [Header("UI Element References")]
    [SerializeField] private Image slotBackground;
    [SerializeField] private Image potionIcon;
    [SerializeField] private GameObject healBadge;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject hotkeyBadge;
    [SerializeField] private TextMeshProUGUI hotkeyText;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Button clickButton;

    [Header("Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip defaultUseSound;

    private float _cooldownTimer = 0f;
    private float _cooldownDuration = 0f;
    private Coroutine _punchCoroutine;
    private Vector3 _originalIconScale = Vector3.one;

    public static QuickPotionSlotUI Instance { get; private set; }

    public PotionItemData ActivePotion
    {
        get => activePotion;
        set
        {
            activePotion = value;
            RefreshDisplay();
        }
    }

    private void Awake()
    {
        Instance = this;

        if (potionIcon != null)
        {
            _originalIconScale = potionIcon.transform.localScale;
        }

        if (clickButton != null)
        {
            clickButton.onClick.AddListener(TryUsePotion);
        }

        if (hotkeyText != null)
        {
            hotkeyText.text = hotkey.ToString();
        }
    }

    private void Start()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChanged += RefreshDisplay;

            if (grantStarterPotionsIfEmpty && activePotion != null)
            {
                if (InventoryManager.Instance.GetItemCount(activePotion) == 0)
                {
                    InventoryManager.Instance.AddItem(activePotion, starterPotionAmount);
                }
            }
        }

        RefreshDisplay();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.onInventoryChanged -= RefreshDisplay;
        }

        if (clickButton != null)
        {
            clickButton.onClick.RemoveListener(TryUsePotion);
        }
    }

    private void Update()
    {
        UpdateCooldown();
        CheckInput();
    }

    private void CheckInput()
    {
        bool hotkeyPressed = false;

        // Check Legacy Input
        if (Input.GetKeyDown(hotkey))
        {
            hotkeyPressed = true;
        }

#if ENABLE_INPUT_SYSTEM
        // Check New Input System
        if (!hotkeyPressed && Keyboard.current != null)
        {
            if (hotkey == KeyCode.Z && Keyboard.current.zKey.wasPressedThisFrame)
            {
                hotkeyPressed = true;
            }
        }
#endif

        if (hotkeyPressed)
        {
            TryUsePotion();
        }
    }

    private void UpdateCooldown()
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f)
            {
                _cooldownTimer = 0f;
                if (cooldownOverlay != null) cooldownOverlay.gameObject.SetActive(false);
                if (cooldownText != null) cooldownText.text = "";
            }
            else
            {
                if (cooldownOverlay != null)
                {
                    cooldownOverlay.gameObject.SetActive(true);
                    cooldownOverlay.fillAmount = _cooldownTimer / _cooldownDuration;
                }
                if (cooldownText != null)
                {
                    cooldownText.text = _cooldownTimer >= 1f ? $"{_cooldownTimer:F1}s" : $"{_cooldownTimer:F1}";
                }
            }
        }
    }

    public void TryUsePotion()
    {
        if (activePotion == null)
            return;

        if (_cooldownTimer > 0f)
            return;

        if (InventoryManager.Instance == null)
            return;

        int currentCount = InventoryManager.Instance.GetItemCount(activePotion);
        if (currentCount <= 0)
        {
            PlayShakeAnimation();
            return;
        }

        PlayerHealth targetHealth = GetActivePlayerHealth();
        if (targetHealth == null)
            return;

        if (!activePotion.CanUse(targetHealth))
        {
            PlayShakeAnimation();
            return;
        }

        // Consume and heal
        bool used = activePotion.Use(targetHealth);
        if (used)
        {
            InventoryManager.Instance.RemoveItem(activePotion, 1);

            // Cooldown
            _cooldownDuration = activePotion.cooldownDuration;
            _cooldownTimer = _cooldownDuration;

            // Audio
            AudioClip sound = activePotion.useSound != null ? activePotion.useSound : defaultUseSound;
            if (sound != null)
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(sound);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(sound, Camera.main != null ? Camera.main.transform.position : transform.position);
                }
            }

            // Punch Animation
            PlayPunchAnimation();
            RefreshDisplay();
        }
    }

    private PlayerHealth GetActivePlayerHealth()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.CurrentCharacterTransform != null)
        {
            var health = PlayerController.Instance.CurrentCharacterTransform.GetComponent<PlayerHealth>();
            if (health != null) return health;
        }

        return Object.FindFirstObjectByType<PlayerHealth>();
    }

    public void RefreshDisplay()
    {
        if (activePotion == null)
        {
            if (potionIcon != null) potionIcon.enabled = false;
            if (countText != null) countText.text = "0";
            if (healBadge != null) healBadge.SetActive(false);
            return;
        }

        if (potionIcon != null)
        {
            potionIcon.enabled = true;
            potionIcon.sprite = activePotion.icon;
        }

        if (healBadge != null)
        {
            healBadge.SetActive(true);
        }

        int count = InventoryManager.Instance != null ? InventoryManager.Instance.GetItemCount(activePotion) : 0;

        if (countText != null)
        {
            countText.text = count.ToString();
        }

        if (potionIcon != null)
        {
            potionIcon.color = count > 0 ? Color.white : new Color(1f, 1f, 1f, 0.4f);
        }
    }

    private void PlayPunchAnimation()
    {
        if (potionIcon == null) return;
        if (_punchCoroutine != null) StopCoroutine(_punchCoroutine);
        _punchCoroutine = StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        float elapsed = 0f;
        float duration = 0.25f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            // Squash and bounce
            float scaleMultiplier = 1f + Mathf.Sin(t * Mathf.PI) * 0.25f;
            potionIcon.transform.localScale = _originalIconScale * scaleMultiplier;
            yield return null;
        }

        potionIcon.transform.localScale = _originalIconScale;
        _punchCoroutine = null;
    }

    private void PlayShakeAnimation()
    {
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector2 origin = rt.anchoredPosition;
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float offsetX = Mathf.Sin(elapsed * 50f) * 6f;
            rt.anchoredPosition = new Vector2(origin.x + offsetX, origin.y);
            yield return null;
        }

        rt.anchoredPosition = origin;
    }
}
