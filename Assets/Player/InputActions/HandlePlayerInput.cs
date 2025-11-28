using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.InputActions
{
    public class HandlePlayerInput : MonoBehaviour
    {
        private PlayerAction _playerAction;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool AimInput { get; private set; }
        public bool SprintInput { get; private set; }

        private void Awake()
        {
            _playerAction = new PlayerAction();
        }

        private void OnEnable()
        {
            // Move (WASD)
            _playerAction.PlayerActions.Move.performed += OnMove;
            _playerAction.PlayerActions.Move.canceled += OnMove;

            // Look (Mouse Delta)
            _playerAction.PlayerActions.Look.performed += OnLook;
            _playerAction.PlayerActions.Look.canceled += OnLook;

            // Jump (Space)
            _playerAction.PlayerActions.Jump.started += OnJump;

            // Aim (Right Mouse Button)
            _playerAction.PlayerActions.Aim.performed += OnAim;
            _playerAction.PlayerActions.Aim.canceled += OnAim;

            // Sprint (Shift)
            _playerAction.PlayerActions.Sprint.performed += OnSprint;
            _playerAction.PlayerActions.Sprint.canceled += OnSprint;

            _playerAction.PlayerActions.Enable();
        }

        private void OnDisable()
        {
            _playerAction.PlayerActions.Move.performed -= OnMove;
            _playerAction.PlayerActions.Move.canceled -= OnMove;

            _playerAction.PlayerActions.Look.performed -= OnLook;
            _playerAction.PlayerActions.Look.canceled -= OnLook;

            _playerAction.PlayerActions.Jump.started -= OnJump;

            _playerAction.PlayerActions.Aim.performed -= OnAim;
            _playerAction.PlayerActions.Aim.canceled -= OnAim;

            _playerAction.PlayerActions.Sprint.performed -= OnSprint;
            _playerAction.PlayerActions.Sprint.canceled -= OnSprint;

            _playerAction.PlayerActions.Disable();
        }

        private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            JumpInput = true;
        }

        private void OnAim(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            AimInput = context.ReadValue<float>() > 0.5f;
        }

        private void OnSprint(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            SprintInput = context.ReadValue<float>() > 0.5f;
        }

        private void LateUpdate()
        {
            JumpInput = false;
        }
    }
}