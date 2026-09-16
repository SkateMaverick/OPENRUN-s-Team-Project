using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MinimapManager : MonoBehaviour
{
    public static MinimapManager Instance { get; private set; }

    [Header("Prefabs & Assets")]
    public GameObject minimapCameraPrefab;
    public RenderTexture minimapRenderTexture;
    public Sprite playerMarkerSprite;
    public Sprite maskSprite;
    public Sprite borderSprite;

    [Header("UI References")]
    public RawImage minimapView;
    public Image frameImage;
    public Image borderImage;
    public Image playerMarkerImage;
    public MinimapPlayerMarker playerMarker;
    public Mask frameMask;
    public Color playerMarkerColor = new Color(0.2f, 0.85f, 1f, 1f);

    [Header("Camera Settings")]
    public float defaultCameraHeight = 30f;
    public float defaultOrthoSize = 20f;

    private MinimapCameraFollow _activeMinimapCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitializeMinimap();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeMinimap();
    }

    public void InitializeMinimap()
    {
        EnsureAssets();
        EnsureMinimapCamera();
        EnsureUI();
    }

    private void EnsureAssets()
    {
        if (minimapRenderTexture == null)
        {
            minimapRenderTexture = Resources.Load<RenderTexture>("MinimapTexture");
        }

        if (minimapCameraPrefab == null)
        {
            minimapCameraPrefab = Resources.Load<GameObject>("MinimapCamera");
        }

        if (playerMarkerSprite == null)
        {
            playerMarkerSprite = Resources.Load<Sprite>("UI/Arrow");
        }

        if (maskSprite == null)
        {
            maskSprite = Resources.Load<Sprite>("UI/MinimapMask");
        }

        if (borderSprite == null)
        {
            borderSprite = Resources.Load<Sprite>("UI/MinimapBorder");
        }
    }

    private void EnsureMinimapCamera()
    {
        _activeMinimapCamera = Object.FindFirstObjectByType<MinimapCameraFollow>();

        if (_activeMinimapCamera == null)
        {
            GameObject camObj = null;

            if (minimapCameraPrefab != null)
            {
                camObj = Instantiate(minimapCameraPrefab);
                camObj.name = "MinimapCamera";
            }
            else
            {
                camObj = new GameObject("MinimapCamera");
                Camera cam = camObj.AddComponent<Camera>();
                cam.orthographic = true;
                cam.orthographicSize = defaultOrthoSize;
                cam.clearFlags = CameraClearFlags.Color;
                cam.backgroundColor = new Color(0.12f, 0.12f, 0.15f, 1f);
                cam.nearClipPlane = 0.3f;
                cam.farClipPlane = 1000f;
                cam.targetTexture = minimapRenderTexture;
                cam.cullingMask = ~0; // Everything
            }

            _activeMinimapCamera = camObj.GetComponent<MinimapCameraFollow>();
            if (_activeMinimapCamera == null)
            {
                _activeMinimapCamera = camObj.AddComponent<MinimapCameraFollow>();
                _activeMinimapCamera.height = defaultCameraHeight;
            }
        }

        if (_activeMinimapCamera != null)
        {
            Camera cam = _activeMinimapCamera.GetComponent<Camera>();
            if (cam != null && minimapRenderTexture != null)
            {
                cam.targetTexture = minimapRenderTexture;
            }
        }
    }

    private void EnsureUI()
    {
        if (frameImage == null)
        {
            frameImage = GetComponent<Image>();
            if (frameImage == null)
            {
                frameImage = gameObject.AddComponent<Image>();
            }
        }

        if (frameImage != null)
        {
            if (maskSprite != null)
            {
                frameImage.sprite = maskSprite;
            }
            frameImage.raycastTarget = false;
        }

        if (frameMask == null)
        {
            frameMask = GetComponent<Mask>();
            if (frameMask == null)
            {
                frameMask = gameObject.AddComponent<Mask>();
            }
        }

        if (frameMask != null)
        {
            frameMask.showMaskGraphic = false;
        }

        if (minimapView == null)
        {
            minimapView = GetComponentInChildren<RawImage>(true);
        }

        if (minimapView != null)
        {
            minimapView.raycastTarget = false;
            if (minimapRenderTexture != null)
            {
                minimapView.texture = minimapRenderTexture;
            }
        }

        if (borderImage == null)
        {
            var borderObj = transform.Find("MinimapBorder");
            if (borderObj != null)
            {
                borderImage = borderObj.GetComponent<Image>();
            }
        }

        if (borderImage == null && borderSprite != null)
        {
            var borderGO = new GameObject("MinimapBorder", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            borderGO.transform.SetParent(transform, false);
            borderImage = borderGO.GetComponent<Image>();
            RectTransform borderRt = borderGO.GetComponent<RectTransform>();
            borderRt.anchorMin = Vector2.zero;
            borderRt.anchorMax = Vector2.one;
            borderRt.sizeDelta = Vector2.zero;
            borderRt.anchoredPosition = Vector2.zero;
        }

        if (borderImage != null)
        {
            if (borderSprite != null)
            {
                borderImage.sprite = borderSprite;
            }
            borderImage.raycastTarget = false;
        }

        if (playerMarkerImage == null)
        {
            var markerObj = transform.Find("PlayerMarker");
            if (markerObj != null)
            {
                playerMarkerImage = markerObj.GetComponent<Image>();
            }
        }

        if (playerMarkerImage != null)
        {
            if (playerMarkerImage.sprite == null && playerMarkerSprite != null)
            {
                playerMarkerImage.sprite = playerMarkerSprite;
            }
            playerMarkerImage.color = playerMarkerColor;
            playerMarkerImage.raycastTarget = false;
            playerMarkerImage.transform.SetAsLastSibling();
        }

        if (playerMarker == null)
        {
            playerMarker = GetComponentInChildren<MinimapPlayerMarker>(true);
            if (playerMarker == null && playerMarkerImage != null)
            {
                playerMarker = playerMarkerImage.gameObject.AddComponent<MinimapPlayerMarker>();
            }
        }
    }
}
