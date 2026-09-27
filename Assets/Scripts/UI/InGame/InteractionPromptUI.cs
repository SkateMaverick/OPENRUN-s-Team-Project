using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image keyBadgeImage;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private Image capsuleImage;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Button promptButton;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.15f;

    private object _currentSource;
    private Coroutine _fadeCoroutine;
    private System.Action _onClickAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        if (promptButton != null)
        {
            promptButton.onClick.AddListener(OnPromptClicked);
        }

        // Initially hidden
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ShowPrompt(string keyName, Sprite icon, string itemName, object source = null, System.Action onClick = null)
    {
        _currentSource = source;
        _onClickAction = onClick;

        if (keyText != null)
        {
            keyText.text = string.IsNullOrEmpty(keyName) ? "F" : keyName;
        }

        if (itemIcon != null)
        {
            if (icon != null)
            {
                itemIcon.sprite = icon;
                itemIcon.enabled = true;
            }
            else
            {
                itemIcon.enabled = false;
            }
        }

        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }

        FadeTo(1f, true);
    }

    public void HidePrompt(object source = null)
    {
        if (source != null && _currentSource != null && _currentSource != source)
        {
            // Another source is active, don't hide
            return;
        }

        _currentSource = null;
        _onClickAction = null;
        FadeTo(0f, false);
    }

    public bool IsShowing => canvasGroup != null && canvasGroup.alpha > 0.05f;

    private void OnPromptClicked()
    {
        _onClickAction?.Invoke();
    }

    private void FadeTo(float targetAlpha, bool interactable)
    {
        if (canvasGroup == null) return;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, interactable));
    }

    private IEnumerator FadeRoutine(float targetAlpha, bool interactable)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = interactable;
        canvasGroup.interactable = interactable;
        _fadeCoroutine = null;
    }
}
