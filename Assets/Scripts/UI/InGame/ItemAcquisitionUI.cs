using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemAcquisitionUI : MonoBehaviour
{
    private static ItemAcquisitionUI _instance;
    public static ItemAcquisitionUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ItemAcquisitionUI>();
                if (_instance == null)
                {
                    Transform targetParent = null;
                    var canvasDesktopGo = GameObject.Find("CanvasDesktop") ?? GameObject.Find("UIPlatformSwitcher/CanvasDesktop");
                    if (canvasDesktopGo != null)
                    {
                        targetParent = canvasDesktopGo.transform;
                    }
                    else
                    {
                        var allCanvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                        foreach (var c in allCanvases)
                        {
                            if (c.renderMode != RenderMode.WorldSpace && c.isActiveAndEnabled)
                            {
                                targetParent = c.transform;
                                break;
                            }
                        }
                    }

                    if (targetParent != null)
                    {
                        var prefab = Resources.Load<GameObject>("UI/ItemAcquisitionPanel");
                        if (prefab != null)
                        {
                            var go = Instantiate(prefab, targetParent, false);
                            _instance = go.GetComponent<ItemAcquisitionUI>();
                        }
                    }
                }
            }
            return _instance;
        }
        private set => _instance = value;
    }

    [Header("UI Root & Container")]
    [SerializeField] private CanvasGroup rootCanvasGroup;
    [SerializeField] private RectTransform containerRect;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Transform rowsParent;
    [SerializeField] private GameObject rowPrefab;

    [Header("Display Settings")]
    [SerializeField] private float displayDuration = 2.8f;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private int maxVisibleRows = 4;
    [SerializeField] private AudioClip defaultAcquisitionSound;

    private class ActiveRow
    {
        public GameObject gameObject;
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        public RectTransform visualTransform;
        public Image iconImage;
        public TextMeshProUGUI text;
        public string itemName;
        public int amount;
        public float timer;
        public Coroutine punchCoroutine;
        public Coroutine fadeCoroutine;
    }

    private readonly List<ActiveRow> _activeRows = new List<ActiveRow>();
    private Coroutine _rootFadeCoroutine;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (rootCanvasGroup == null)
        {
            rootCanvasGroup = GetComponent<CanvasGroup>();
            if (rootCanvasGroup == null)
            {
                rootCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Initially hide root
        rootCanvasGroup.alpha = 0f;
        rootCanvasGroup.blocksRaycasts = false;
        rootCanvasGroup.interactable = false;

        if (rowPrefab != null)
        {
            rowPrefab.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateRowsTimer();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void HandleItemAcquired(ItemData item, int amount)
    {
        if (item == null || amount <= 0) return;
        ShowItemAcquired(item.icon, item.itemName, amount);
    }

    public static void Show(ItemData item, int amount = 1)
    {
        if (Instance != null)
        {
            Instance.ShowItemAcquired(item, amount);
        }
    }

    public static void Show(Sprite icon, string itemName, int amount = 1)
    {
        if (Instance != null)
        {
            Instance.ShowItemAcquired(icon, itemName, amount);
        }
    }

    public void ShowItemAcquired(ItemData item, int amount = 1)
    {
        if (item == null) return;
        ShowItemAcquired(item.icon, item.itemName, amount);
    }

    public void ShowItemAcquired(Sprite icon, string itemName, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemName) || amount <= 0) return;

        // Check if a row for this item already exists
        ActiveRow existingRow = _activeRows.Find(r => r != null && r.itemName == itemName);
        if (existingRow != null && existingRow.gameObject != null)
        {
            existingRow.amount += amount;
            existingRow.text.text = $"{existingRow.itemName} × {existingRow.amount}";
            existingRow.timer = displayDuration;

            // Trigger punch animation on existing row
            if (existingRow.punchCoroutine != null)
            {
                StopCoroutine(existingRow.punchCoroutine);
            }
            existingRow.punchCoroutine = StartCoroutine(PunchRowRoutine(existingRow.visualTransform ?? existingRow.rectTransform));

            // Ensure row is fully visible if it was fading out
            if (existingRow.fadeCoroutine != null)
            {
                StopCoroutine(existingRow.fadeCoroutine);
                existingRow.fadeCoroutine = null;
            }
            existingRow.canvasGroup.alpha = 1f;
        }
        else
        {
            // If at max rows, force-remove oldest
            if (_activeRows.Count >= maxVisibleRows)
            {
                RemoveRowImmediately(_activeRows[0]);
            }

            // Spawn new row
            SpawnRow(icon, itemName, amount);
        }

        // Play sound if assigned
        if (defaultAcquisitionSound != null)
        {
            AudioSource.PlayClipAtPoint(defaultAcquisitionSound, Camera.main != null ? Camera.main.transform.position : Vector3.zero, 0.7f);
        }

        // Show root panel
        SetRootVisible(true);
    }

    private void SpawnRow(Sprite icon, string itemName, int amount)
    {
        if (rowPrefab == null || rowsParent == null) return;

        GameObject rowGo = Instantiate(rowPrefab, rowsParent);
        rowGo.SetActive(true);

        RectTransform rt = rowGo.GetComponent<RectTransform>();
        CanvasGroup cg = rowGo.GetComponent<CanvasGroup>();
        if (cg == null) cg = rowGo.AddComponent<CanvasGroup>();

        Transform visualT = rowGo.transform.Find("VisualContent");
        RectTransform visualRt = visualT != null ? visualT.GetComponent<RectTransform>() : rt;

        // Find icon and text
        Image img = null;
        TextMeshProUGUI txt = null;

        Transform iconT = rowGo.transform.Find("VisualContent/ItemIcon") ?? rowGo.transform.Find("ItemIcon");
        if (iconT != null) img = iconT.GetComponent<Image>();
        else img = rowGo.GetComponentInChildren<Image>();

        Transform textT = rowGo.transform.Find("VisualContent/ItemText") ?? rowGo.transform.Find("ItemText");
        if (textT != null) txt = textT.GetComponent<TextMeshProUGUI>();
        else txt = rowGo.GetComponentInChildren<TextMeshProUGUI>();

        if (img != null)
        {
            if (icon != null)
            {
                img.sprite = icon;
                img.enabled = true;
            }
            else
            {
                img.enabled = false;
            }
        }

        if (txt != null)
        {
            txt.text = $"{itemName} × {amount}";
        }

        ActiveRow newRow = new ActiveRow
        {
            gameObject = rowGo,
            canvasGroup = cg,
            rectTransform = rt,
            visualTransform = visualRt,
            iconImage = img,
            text = txt,
            itemName = itemName,
            amount = amount,
            timer = displayDuration
        };

        _activeRows.Add(newRow);

        // Animate row entrance (slide & fade)
        StartCoroutine(AnimateRowIn(newRow));
    }

    private IEnumerator AnimateRowIn(ActiveRow row)
    {
        if (row == null || row.gameObject == null) yield break;

        row.canvasGroup.alpha = 0f;
        RectTransform targetRt = row.visualTransform ?? row.rectTransform;
        Vector2 originalPos = Vector2.zero;
        Vector2 startPos = new Vector2(-30f, 0f);
        targetRt.anchoredPosition = startPos;

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            if (row == null || row.gameObject == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float ease = 1f - Mathf.Pow(1f - t, 3f); // EaseOutCubic

            row.canvasGroup.alpha = t;
            targetRt.anchoredPosition = Vector2.LerpUnclamped(startPos, originalPos, ease);
            yield return null;
        }

        if (row != null && row.gameObject != null)
        {
            row.canvasGroup.alpha = 1f;
            targetRt.anchoredPosition = originalPos;
        }
    }

    private IEnumerator PunchRowRoutine(RectTransform rt)
    {
        if (rt == null) yield break;

        float elapsed = 0f;
        float duration = 0.18f;

        while (elapsed < duration)
        {
            if (rt == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Scale up to 1.08 then ease back to 1.0
            float s = 1f + Mathf.Sin(t * Mathf.PI) * 0.08f;
            rt.localScale = new Vector3(s, s, 1f);
            yield return null;
        }

        if (rt != null)
        {
            rt.localScale = Vector3.one;
        }
    }

    private void UpdateRowsTimer()
    {
        if (_activeRows.Count == 0) return;

        for (int i = _activeRows.Count - 1; i >= 0; i--)
        {
            ActiveRow row = _activeRows[i];
            if (row == null || row.gameObject == null)
            {
                _activeRows.RemoveAt(i);
                continue;
            }

            row.timer -= Time.deltaTime;
            if (row.timer <= 0f && row.fadeCoroutine == null)
            {
                row.fadeCoroutine = StartCoroutine(FadeOutRowRoutine(row));
            }
        }
    }

    private IEnumerator FadeOutRowRoutine(ActiveRow row)
    {
        if (row == null || row.gameObject == null) yield break;

        float startAlpha = row.canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            if (row == null || row.gameObject == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            row.canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        RemoveRowImmediately(row);

        // If no more rows, hide root panel
        if (_activeRows.Count == 0)
        {
            SetRootVisible(false);
        }
    }

    private void RemoveRowImmediately(ActiveRow row)
    {
        if (row == null) return;
        _activeRows.Remove(row);
        if (row.gameObject != null)
        {
            Destroy(row.gameObject);
        }
    }

    private void SetRootVisible(bool visible)
    {
        if (rootCanvasGroup == null) return;

        if (_rootFadeCoroutine != null)
        {
            StopCoroutine(_rootFadeCoroutine);
        }

        _rootFadeCoroutine = StartCoroutine(FadeRootRoutine(visible ? 1f : 0f));
    }

    private IEnumerator FadeRootRoutine(float targetAlpha)
    {
        float startAlpha = rootCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            rootCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        rootCanvasGroup.alpha = targetAlpha;
        rootCanvasGroup.blocksRaycasts = false;
        rootCanvasGroup.interactable = false;
    }
}
