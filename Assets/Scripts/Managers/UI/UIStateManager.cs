using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class UIStateManager : MonoBehaviour
{
    public static UIStateManager Instance { get; private set; }

    public bool IsAnyUIOpen => _openUIKeys.Count > 0;
    public bool IsAltHeld { get; private set; }
    public bool IsCursorActive => IsAnyUIOpen || IsAltHeld;

    public event Action<bool> OnUIStateChanged;

    private readonly HashSet<string> _openUIKeys = new HashSet<string>();
    private readonly List<CinemachineInputAxisController> _disabledControllers = new List<CinemachineInputAxisController>();
    private bool _isCameraBlocked;

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

    private void Update()
    {
        bool altPressed = CheckLeftAltPressed();
        if (altPressed != IsAltHeld)
        {
            IsAltHeld = altPressed;
            RefreshState();
        }

        if (_isCameraBlocked)
        {
            KeepControllersBlocked();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && IsAltHeld)
        {
            IsAltHeld = false;
            RefreshState();
        }
    }

    public void OpenUI(string uiKey)
    {
        bool wasActive = IsCursorActive;
        _openUIKeys.Add(uiKey);

        if (!wasActive && IsCursorActive)
        {
            RefreshState();
        }
    }

    public void CloseUI(string uiKey)
    {
        bool wasActive = IsCursorActive;
        _openUIKeys.Remove(uiKey);

        if (wasActive && !IsCursorActive)
        {
            RefreshState();
        }
    }

    public void SetUIState(string uiKey, bool isOpen)
    {
        if (isOpen)
            OpenUI(uiKey);
        else
            CloseUI(uiKey);
    }

    private void RefreshState()
    {
        bool active = IsCursorActive;
        ApplyCursorState(active);
        SetCameraInputBlocked(active);
        OnUIStateChanged?.Invoke(active);
    }

    private void ApplyCursorState(bool cursorActive)
    {
        Cursor.visible = cursorActive;
        Cursor.lockState = cursorActive ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void SetCameraInputBlocked(bool blocked)
    {
        if (blocked)
        {
            if (!_isCameraBlocked)
            {
                _disabledControllers.Clear();
                _isCameraBlocked = true;
            }

            KeepControllersBlocked();
        }
        else
        {
            if (_isCameraBlocked)
            {
                foreach (var controller in _disabledControllers)
                {
                    if (controller != null)
                    {
                        controller.enabled = true;
                    }
                }
                _disabledControllers.Clear();
                _isCameraBlocked = false;
            }
        }
    }

    private void KeepControllersBlocked()
    {
        var controllers = UnityEngine.Object.FindObjectsByType<CinemachineInputAxisController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var controller in controllers)
        {
            if (controller != null && controller.enabled)
            {
                controller.enabled = false;
                if (!_disabledControllers.Contains(controller))
                {
                    _disabledControllers.Add(controller);
                }
            }
        }
    }

    private bool CheckLeftAltPressed()
    {
        bool isPressed = false;

        if (Keyboard.current != null)
        {
            isPressed = Keyboard.current.leftAltKey.isPressed;
        }

        if (!isPressed)
        {
            isPressed = Input.GetKey(KeyCode.LeftAlt);
        }

        return isPressed;
    }

    private void OnDisable()
    {
        if (_isCameraBlocked)
        {
            SetCameraInputBlocked(false);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (_isCameraBlocked)
        {
            SetCameraInputBlocked(false);
        }
    }
}