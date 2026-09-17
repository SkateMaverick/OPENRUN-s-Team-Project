using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterScaleUI : MonoBehaviour
{
    private const string UI_KEY = "CharacterScaleUI";

    [Header("UI References")]
    public GameObject scalePanel;
    public Slider scaleSlider;
    public TextMeshProUGUI scaleValueText;
    public Button backButton;

    [Header("Target Character (Noa Only)")]
    public Transform targetCharacter;

    public bool IsOpen => scalePanel != null && scalePanel.activeSelf;

    private void Awake()
    {
        EnsureUIReferences();
    }

    private void Start()
    {
        EnsureUIReferences();

        if (scalePanel != null)
        {
            scalePanel.SetActive(false);
        }

        EnsureTargetCharacter();

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
            PlayerController.Instance.OnCharacterChanged += OnCharacterChanged;
        }

        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.RemoveListener(SetCharacterScale);
            scaleSlider.onValueChanged.AddListener(SetCharacterScale);
            SetCharacterScale(scaleSlider.value);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(ClosePanel);
            backButton.onClick.AddListener(ClosePanel);
        }
    }

    private void OnEnable()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
            PlayerController.Instance.OnCharacterChanged += OnCharacterChanged;
        }
    }

    private void OnDisable()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
        }

        if (IsOpen)
        {
            ClosePanel();
        }
    }

    private void OnDestroy()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.OnCharacterChanged -= OnCharacterChanged;
        }

        if (IsOpen && UIStateManager.Instance != null)
        {
            UIStateManager.Instance.CloseUI(UI_KEY);
        }
    }

    private void OnCharacterChanged(Transform currentCharacterTransform)
    {
        // If switched to Que or another character while menu is open, close it
        if (!IsNoaSelected() && IsOpen)
        {
            ClosePanel();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TogglePanel();
        }
    }

    /// <summary>
    /// Checks if the currently active / controlled character is Noa (BoxGolem).
    /// </summary>
    public bool IsNoaSelected()
    {
        var pc = PlayerController.Instance != null ? PlayerController.Instance : Object.FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            return pc.CurrentCharacterType == Enums.CharacterType.BoxGolem;
        }

        EnsureTargetCharacter();
        return targetCharacter != null;
    }

    private void EnsureUIReferences()
    {
        if (scalePanel == null)
        {
            var panelT = transform.Find("ScalePanel");
            if (panelT != null)
            {
                scalePanel = panelT.gameObject;
            }
        }

        if (scalePanel != null)
        {
            if (scaleSlider == null)
            {
                scaleSlider = scalePanel.GetComponentInChildren<Slider>(true);
            }

            if (scaleValueText == null)
            {
                var valObj = scalePanel.transform.Find("ScaleValueText");
                if (valObj != null)
                {
                    scaleValueText = valObj.GetComponent<TextMeshProUGUI>();
                }
            }

            if (backButton == null)
            {
                var btnObj = scalePanel.transform.Find("BackButton");
                if (btnObj != null)
                {
                    backButton = btnObj.GetComponent<Button>();
                }
            }
        }
    }

    public void OpenPanel()
    {
        // Only open the menu if Noa is the currently active character, or switch to Noa
        if (!IsNoaSelected())
        {
            var pc = PlayerController.Instance != null ? PlayerController.Instance : Object.FindFirstObjectByType<PlayerController>();
            if (pc != null)
            {
                pc.SwitchToBoxGolem();
            }
            else
            {
                return;
            }
        }

        EnsureTargetCharacter();

        if (scalePanel != null)
        {
            scalePanel.SetActive(true);
        }

        var uiManager = UIStateManager.Instance != null ? UIStateManager.Instance : Object.FindFirstObjectByType<UIStateManager>();
        if (uiManager != null)
        {
            uiManager.OpenUI(UI_KEY);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ClosePanel()
    {
        if (scalePanel != null)
        {
            scalePanel.SetActive(false);
        }

        var uiManager = UIStateManager.Instance != null ? UIStateManager.Instance : Object.FindFirstObjectByType<UIStateManager>();
        if (uiManager != null)
        {
            uiManager.CloseUI(UI_KEY);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void TogglePanel()
    {
        if (IsOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    /// <summary>
    /// Ensures that only Noa (the character with ScaleChange) is targeted.
    /// Que (SphereGolem) has no scale mechanics and must not be targeted.
    /// </summary>
    private void EnsureTargetCharacter()
    {
        // 1. If current target already has ScaleChange, it's valid
        if (targetCharacter != null && targetCharacter.GetComponent<ScaleChange>() != null)
        {
            return;
        }

        // 2. Check PlayerController's boxGolem (Noa)
        if (PlayerController.Instance != null && PlayerController.Instance.boxGolem != null)
        {
            targetCharacter = PlayerController.Instance.boxGolem.transform;
            return;
        }

        // 3. Find any ScaleChange in the scene (Noa's component)
        var scaleChangeObj = Object.FindFirstObjectByType<ScaleChange>();
        if (scaleChangeObj != null)
        {
            targetCharacter = scaleChangeObj.transform;
            return;
        }

        // 4. Find GameObject named "Noa"
        var noa = GameObject.Find("Noa");
        if (noa != null)
        {
            targetCharacter = noa.transform;
        }
    }

    /// <summary>
    /// Adjusts the scale blend shapes and collider of Noa.
    /// Never modifies Que or scales arbitrary transforms.
    /// </summary>
    public void SetCharacterScale(float scaleValue)
    {
        EnsureTargetCharacter();
        if (targetCharacter == null) return;

        var scaleChange = targetCharacter.GetComponent<ScaleChange>();
        if (scaleChange != null)
        {
            scaleChange.OnSliderValueChanged(scaleValue);
        }
        else
        {
            var netScaleChange = targetCharacter.GetComponent<NetworkScaleChange>();
            if (netScaleChange != null)
            {
                netScaleChange.OnSliderValueChanged(scaleValue);
            }
        }

        if (scaleValueText != null)
        {
            scaleValueText.text = $"{scaleValue:F2}";
        }
    }
}
