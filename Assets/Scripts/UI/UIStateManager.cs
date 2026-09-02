using System;
using System.Collections.Generic;
using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    public static UIStateManager Instance { get; private set; }

    public bool IsAnyUIOpen => _openUIKeys.Count > 0;

    public event Action<bool> OnUIStateChanged;

    private readonly HashSet<string> _openUIKeys = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ApplyCursorState(false);
    }

    public void OpenUI(string uiKey)
    {
        bool wasOpen = IsAnyUIOpen;

        _openUIKeys.Add(uiKey);

        if (!wasOpen && IsAnyUIOpen)
        {
            ApplyCursorState(true);
            OnUIStateChanged?.Invoke(true);
        }
    }

    public void CloseUI(string uiKey)
    {
        bool wasOpen = IsAnyUIOpen;

        _openUIKeys.Remove(uiKey);

        if (wasOpen && !IsAnyUIOpen)
        {
            ApplyCursorState(false);
            OnUIStateChanged?.Invoke(false);
        }
    }

    public void SetUIState(string uiKey, bool isOpen)
    {
        if (isOpen)
            OpenUI(uiKey);
        else
            CloseUI(uiKey);
    }

    private void ApplyCursorState(bool uiOpen)
    {
        Cursor.visible = uiOpen;
        Cursor.lockState = uiOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}