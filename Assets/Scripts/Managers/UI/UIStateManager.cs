using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class UIStateManager : MonoBehaviour
{
    public static UIStateManager Instance { get; private set; }

    public bool IsAnyUIOpen => _openUIKeys.Count > 0;

    public event Action<bool> OnUIStateChanged;

    private readonly HashSet<string> _openUIKeys = new HashSet<string>();
    private readonly List<CinemachineInputAxisController> _disabledControllers = new List<CinemachineInputAxisController>();

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
            SetCameraInputBlocked(true);
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
            SetCameraInputBlocked(false);
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

    private void SetCameraInputBlocked(bool blocked)
    {
        if (blocked)
        {
            _disabledControllers.Clear();
            var controllers = UnityEngine.Object.FindObjectsByType<CinemachineInputAxisController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var controller in controllers)
            {
                if (controller != null && controller.enabled)
                {
                    controller.enabled = false;
                    _disabledControllers.Add(controller);
                }
            }
        }
        else
        {
            foreach (var controller in _disabledControllers)
            {
                if (controller != null)
                {
                    controller.enabled = true;
                }
            }
            _disabledControllers.Clear();
        }
    }
}