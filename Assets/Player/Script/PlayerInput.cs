using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Script
{
    public class PlayerInput : MonoBehaviour
    {
        private PlayerAction _playerAction;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; } // --- 1. LookInput 추가 ---
        public bool JumpInput { get; private set; }
        public bool AimInput { get; private set; }
        public bool SprintInput { get; private set; }

        private void Awake()
        {
            _playerAction = new PlayerAction();
        }

        private void OnEnable()
        {
            _playerAction.PlayerActions.Move.performed += OnMove;
            _playerAction.PlayerActions.Move.canceled += OnMove;
            _playerAction.PlayerActions.Jump.started += OnJump;
            _playerAction.PlayerActions.Aim.performed += OnAim;
            _playerAction.PlayerActions.Aim.canceled += OnAim;
            _playerAction.PlayerActions.Sprint.performed += OnSprint;
            _playerAction.PlayerActions.Sprint.canceled += OnSprint;
            _playerAction.PlayerActions.Look.performed += OnLook;
            _playerAction.PlayerActions.Look.canceled += OnLook;

            _playerAction.PlayerActions.Enable();
        }

        private void OnDisable()
        {
            _playerAction.PlayerActions.Move.performed -= OnMove;
            _playerAction.PlayerActions.Move.canceled -= OnMove;
            _playerAction.PlayerActions.Jump.started -= OnJump;
            _playerAction.PlayerActions.Aim.performed -= OnAim;
            _playerAction.PlayerActions.Aim.canceled -= OnAim;
            _playerAction.PlayerActions.Sprint.performed -= OnSprint;
            _playerAction.PlayerActions.Sprint.canceled -= OnSprint;
            _playerAction.PlayerActions.Look.performed -= OnLook;
            _playerAction.PlayerActions.Look.canceled -= OnLook;

            _playerAction.PlayerActions.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        // --- 4. Look 콜백 메서드 추가 ---
        private void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }
        // -------------------------

        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                JumpInput = true;
            }
        }

        private void OnAim(InputAction.CallbackContext context)
        {
            AimInput = context.ReadValue<float>() > 0.5f;
        }

        private void OnSprint(InputAction.CallbackContext context)
        {
            SprintInput = context.ReadValue<float>() > 0.5f;
        }

        private void LateUpdate()
        {
            JumpInput = false;
        }
    }
}