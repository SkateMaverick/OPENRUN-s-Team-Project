using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterScaleUI : MonoBehaviour
{
    public GameObject scalePanel;
    public Transform targetCharacter;
    public Slider scaleSlider;
    public TextMeshProUGUI scaleValueText;

    private void Start()
    {
        if (scalePanel != null)
        {
            scalePanel.SetActive(false);
        }

        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.AddListener(SetCharacterScale);
            SetCharacterScale(scaleSlider.value);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TogglePanel();
        }
    }

    public void OpenPanel()
    {
        if (scalePanel != null)
        {
            scalePanel.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ClosePanel()
    {
        if (scalePanel != null)
        {
            scalePanel.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void TogglePanel()
    {
        if (scalePanel != null)
        {
            bool isOpen = !scalePanel.activeSelf;
            scalePanel.SetActive(isOpen);

            if (isOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    public void SetCharacterScale(float scaleValue)
    {
        if (targetCharacter == null) return;

        targetCharacter.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

        if (scaleValueText != null)
        {
            scaleValueText.text = $"Å©±â: {scaleValue:F2}";
        }
    }
}