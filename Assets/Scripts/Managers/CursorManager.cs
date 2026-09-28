using UnityEngine;
using Enums;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Vector2 defaultHotspot = Vector2.zero;

    [SerializeField] private Texture2D hoverCursor;
    [SerializeField] private Vector2 hoverHotspot = Vector2.zero;

    private Texture2D _currentCustomTexture;
    private Vector2 _currentCustomHotspot;
    private GameCursorType _currentCursorType = GameCursorType.Default;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ApplyCursor();
    }

    public void SetCursorType(GameCursorType type)
    {
        _currentCursorType = type;
        ApplyCursor();
    }

    public void SetCustomCursor(Texture2D texture, Vector2 hotspot)
    {
        _currentCursorType = GameCursorType.Custom;
        _currentCustomTexture = texture;
        _currentCustomHotspot = hotspot;
        ApplyCursor();
    }

    public void ClearForcedCursor()
    {
        _currentCursorType = GameCursorType.Default;
        _currentCustomTexture = null;
        ApplyCursor();
    }

    private void ApplyCursor()
    {
        switch (_currentCursorType)
        {
            case GameCursorType.Hover:
                if (hoverCursor != null)
                {
                    Cursor.SetCursor(hoverCursor, hoverHotspot, CursorMode.Auto);
                    return;
                }
                break;

            case GameCursorType.Custom:
                if (_currentCustomTexture != null)
                {
                    Cursor.SetCursor(_currentCustomTexture, _currentCustomHotspot, CursorMode.Auto);
                    return;
                }
                break;

            case GameCursorType.Default:
            default:
                if (defaultCursor != null)
                {
                    Cursor.SetCursor(defaultCursor, defaultHotspot, CursorMode.Auto);
                    return;
                }
                break;
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
