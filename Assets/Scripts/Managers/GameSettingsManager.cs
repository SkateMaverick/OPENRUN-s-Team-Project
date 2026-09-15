using System;
using UnityEngine;
using Enums;

public static class GameSettingsManager
{
    private const string PREF_MOUSE_SENSITIVITY = "Settings_MouseSensitivity";
    private const string PREF_GRAPHIC_QUALITY = "Settings_GraphicQuality";

    public const float DEFAULT_SENSITIVITY = 1.0f;
    public const GraphicQualityLevel DEFAULT_QUALITY = GraphicQualityLevel.High;

    public static event Action<float> OnMouseSensitivityChanged;
    public static event Action<GraphicQualityLevel> OnGraphicQualityChanged;

    private static float _mouseSensitivity = -1f;
    private static GraphicQualityLevel _graphicQuality = (GraphicQualityLevel)(-1);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSettings()
    {
        LoadSettings();
        ApplyGraphicQuality(_graphicQuality);
    }

    public static float MouseSensitivity
    {
        get
        {
            if (_mouseSensitivity < 0f)
            {
                LoadSettings();
            }
            return _mouseSensitivity;
        }
        set
        {
            float clamped = Mathf.Clamp(value, 0.1f, 3.0f);
            if (!Mathf.Approximately(_mouseSensitivity, clamped))
            {
                _mouseSensitivity = clamped;
                PlayerPrefs.SetFloat(PREF_MOUSE_SENSITIVITY, _mouseSensitivity);
                PlayerPrefs.Save();
                OnMouseSensitivityChanged?.Invoke(_mouseSensitivity);
            }
        }
    }

    public static GraphicQualityLevel GraphicQuality
    {
        get
        {
            if ((int)_graphicQuality < 0)
            {
                LoadSettings();
            }
            return _graphicQuality;
        }
        set
        {
            if (_graphicQuality != value)
            {
                _graphicQuality = value;
                PlayerPrefs.SetInt(PREF_GRAPHIC_QUALITY, (int)_graphicQuality);
                PlayerPrefs.Save();
                ApplyGraphicQuality(_graphicQuality);
                OnGraphicQualityChanged?.Invoke(_graphicQuality);
            }
        }
    }

    private static void LoadSettings()
    {
        _mouseSensitivity = PlayerPrefs.GetFloat(PREF_MOUSE_SENSITIVITY, DEFAULT_SENSITIVITY);
        _graphicQuality = (GraphicQualityLevel)PlayerPrefs.GetInt(PREF_GRAPHIC_QUALITY, (int)DEFAULT_QUALITY);
    }

    private static void ApplyGraphicQuality(GraphicQualityLevel quality)
    {
        int qualityLevelsCount = QualitySettings.names.Length;
        int targetQualityIndex = 0;

        if (qualityLevelsCount >= 6)
        {
            switch (quality)
            {
                case GraphicQualityLevel.Low:
                    targetQualityIndex = 1;
                    break;
                case GraphicQualityLevel.Middle:
                    targetQualityIndex = 2;
                    break;
                case GraphicQualityLevel.High:
                default:
                    targetQualityIndex = 5;
                    break;
            }
        }
        else if (qualityLevelsCount >= 3)
        {
            targetQualityIndex = Mathf.Clamp((int)quality, 0, qualityLevelsCount - 1);
        }
        else if (qualityLevelsCount > 0)
        {
            targetQualityIndex = Mathf.Clamp((int)quality, 0, qualityLevelsCount - 1);
        }

        QualitySettings.SetQualityLevel(targetQualityIndex, true);
    }
}
