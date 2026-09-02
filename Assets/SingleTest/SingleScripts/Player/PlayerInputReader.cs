using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Player.InputActions
{
    public class PlayerInputReader : MonoBehaviour
    {
        // 싱글톤 변수
        public static PlayerInputReader Instance { get; private set; }
        
        private PlayerAction _playerAction;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool FireInput { get; private set; }
        public bool SprintInput { get; private set; }

        private readonly HashSet<int> _lookFingers = new HashSet<int>();

        private void Awake()
        {
            _playerAction = new PlayerAction();
            
            #region 싱글톤
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion
        }

        private void OnEnable()
        {
            _playerAction.PlayerActions.Move.performed += OnMove;
            _playerAction.PlayerActions.Move.canceled += OnMove;

            _playerAction.PlayerActions.Look.performed += OnLook;
            _playerAction.PlayerActions.Look.canceled += OnLook;

            _playerAction.PlayerActions.Jump.started += OnJump;

            _playerAction.PlayerActions.Shot.performed += OnAim;
            _playerAction.PlayerActions.Shot.canceled += OnAim;

            _playerAction.PlayerActions.Sprint.performed += OnSprint;
            _playerAction.PlayerActions.Sprint.canceled += OnSprint;

            _playerAction.PlayerActions.Enable();
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            _playerAction.PlayerActions.Move.performed -= OnMove;
            _playerAction.PlayerActions.Move.canceled -= OnMove;

            _playerAction.PlayerActions.Look.performed -= OnLook;
            _playerAction.PlayerActions.Look.canceled -= OnLook;

            _playerAction.PlayerActions.Jump.started -= OnJump;

            _playerAction.PlayerActions.Shot.performed -= OnAim;
            _playerAction.PlayerActions.Shot.canceled -= OnAim;

            _playerAction.PlayerActions.Sprint.performed -= OnSprint;
            _playerAction.PlayerActions.Sprint.canceled -= OnSprint;

            _playerAction.PlayerActions.Disable();
            EnhancedTouchSupport.Disable();
        }

        private bool IsUsingCursorUI()
        {
            return UIStateManager.Instance != null && UIStateManager.Instance.IsAnyUIOpen;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (IsUsingCursorUI())
            {
                MoveInput = Vector2.zero;
                return;
            }

            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            if (IsUsingCursorUI())
            {
                LookInput = Vector2.zero;
                return;
            }

            LookInput = context.ReadValue<Vector2>();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (IsUsingCursorUI())
            {
                JumpInput = false;
                return;
            }

            JumpInput = true;
        }

        private void OnAim(InputAction.CallbackContext context)
        {
            if (IsUsingCursorUI())
            {
                FireInput = false;
                return;
            }

            FireInput = context.ReadValue<float>() > 0.5f;
        }

        private void OnSprint(InputAction.CallbackContext context)
        {
            if (IsUsingCursorUI())
            {
                SprintInput = false;
                return;
            }

            SprintInput = context.ReadValue<float>() > 0.5f;
        }

        private void Update()
        {
            if (IsUsingCursorUI())
            {
                MoveInput = Vector2.zero;
                LookInput = Vector2.zero;
                FireInput = false;
                SprintInput = false;
                return;
            }

            if (CurrentDevice.IsMobile)
            {
                HandleMobileLook();
            }
        }

        private void LateUpdate()
        {
            JumpInput = false;
        }

        #region 모바일 처리

        private void HandleMobileLook()
        {
            LookInput = Vector2.zero;

            foreach (var touch in ETouch.activeTouches)
            {
                int fingerIndex = touch.finger.index;

                switch (touch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        if (!IsPointerOverUI(touch))
                        {
                            _lookFingers.Add(fingerIndex);
                        }
                        break;

                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        if (_lookFingers.Contains(fingerIndex))
                        {
                            LookInput += touch.delta;
                        }
                        break;

                    case UnityEngine.InputSystem.TouchPhase.Ended:
                    case UnityEngine.InputSystem.TouchPhase.Canceled:
                        _lookFingers.Remove(fingerIndex);
                        break;
                }
            }
        }

        private bool IsPointerOverUI(ETouch touch)
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = touch.screenPosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }

        #endregion
    }
}