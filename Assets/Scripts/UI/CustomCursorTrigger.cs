using UnityEngine;
using UnityEngine.EventSystems;
using Enums;

public class CustomCursorTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameCursorType _hoverCursorType = GameCursorType.Hover;
    [SerializeField] private Texture2D _customCursorTexture;
    [SerializeField] private Vector2 _customHotspot = Vector2.zero;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverCursorType == GameCursorType.Custom && _customCursorTexture != null)
        {
            CursorManager.Instance.SetCustomCursor(_customCursorTexture, _customHotspot);
        }
        else
        {
            CursorManager.Instance.SetCursorType(_hoverCursorType);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.ClearForcedCursor();
    }

    private void OnDisable()
    {
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.ClearForcedCursor();
        }
    }
}
