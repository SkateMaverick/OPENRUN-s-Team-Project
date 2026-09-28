using UnityEngine;
using Player.InputActions;
using Photon.Pun;

namespace Player.Script
{
    public class LegacyNetworkPlayerMovement : MonoBehaviourPun
    {
        public float moveSpeed = 4.0f;
        private Vector3 _lastMoveDirection;

        public float gravity = -30f;
        public float currentVelocity;
        public float maxFallSpeed = 53.0f;

        [Header("Ground Settings")]
        public bool grounded = true;
        public LayerMask groundLayers;

        private CharacterController _controller;
        private Transform _mainCameraTransform;
        private NetworkPlayerInput _playerInput;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            
            if (Camera.main != null)
            {
                _mainCameraTransform = Camera.main.transform;
            }

            _playerInput = GetComponent<NetworkPlayerInput>();
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            GroundedCheck();
            Gravity();
            Move();
        }

        private void Move()
        {
            if (_playerInput == null || _mainCameraTransform == null) return;

            float h = _playerInput.MoveInput.x;
            float v = _playerInput.MoveInput.y;
            
            Vector3 inputVector = new Vector3(h, 0f, v).normalized;
            Quaternion cameraRotation = Quaternion.Euler(0, _mainCameraTransform.eulerAngles.y, 0);

            _lastMoveDirection = cameraRotation * inputVector;
            
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hitInfo, 1.0f, groundLayers, QueryTriggerInteraction.Ignore))
            {
                if (grounded)
                {
                    _lastMoveDirection = Vector3.ProjectOnPlane(_lastMoveDirection, hitInfo.normal).normalized;
                }
            }

            Quaternion targetRotation = Quaternion.Euler(0, _mainCameraTransform.eulerAngles.y, 0);
            transform.rotation = targetRotation;
            
            float finalHorizontalSpeed;

            if (h != 0 || v != 0)
            {
                bool isSprinting = _playerInput.SprintInput && !_playerInput.AimInput;
                finalHorizontalSpeed = isSprinting ? moveSpeed * 2.0f : moveSpeed;
            }
            else
            {
                finalHorizontalSpeed = 0f;
            }
            
            Vector3 finalVelocity = (_lastMoveDirection * finalHorizontalSpeed) + (Vector3.up * currentVelocity);
            _controller.Move(finalVelocity * Time.deltaTime);
        }

        private void Gravity()
        {
            if (grounded)
            {
                if (currentVelocity < 0.0f)
                {
                    currentVelocity = -10f;
                }
            }
            else
            {
                if (currentVelocity > -maxFallSpeed)
                {
                    currentVelocity += gravity * Time.deltaTime;
                }
            }
        }
        
        private void GroundedCheck()
        {
            if (_controller != null)
            {
                grounded = _controller.isGrounded;
            }
        }
    }
}