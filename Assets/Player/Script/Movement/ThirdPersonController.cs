using UnityEngine;
using Player.InputActions;

namespace Player.Script
{
    public class ThirdPersonController : MonoBehaviour
    {
        #region Movement Variables
        public float moveSpeed = 4.0f;
        private Vector3 _lastMoveDirection;
        #endregion

        #region Gravity Variables
        public float gravity = -30f;
        public float currentVelocity;
        public float maxFallSpeed = 53.0f;
        #endregion

        #region Ground Variables
        public bool grounded = true;
        public float groundedOffset = 0.1f;
        public float groundedRadius = 0.2f;
        public LayerMask groundLayers;
        #endregion

        #region Player Variables
        private CharacterController _controller;
        private Transform _mainCameraTransform;
        private HandlePlayerInput _playerInput;
        #endregion

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _mainCameraTransform = Camera.main.transform;
            _playerInput = GetComponent<HandlePlayerInput>();
        }

        private void Update()
        {
            GroundedCheck();
            Gravity();
            Move();
        }

        private void Move()
        {
            float h = _playerInput.MoveInput.x;
            float v = _playerInput.MoveInput.y;
            
            Vector3 inputVector = new Vector3(h, 0f, v).normalized;
            Quaternion cameraRotation = Quaternion.Euler(0, _mainCameraTransform.eulerAngles.y, 0);

            _lastMoveDirection = cameraRotation * inputVector;
            
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
                    currentVelocity = -2f;
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
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundedOffset,
                transform.position.z);
                
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundedOffset,
                transform.position.z);

            Gizmos.DrawWireSphere(spherePosition, groundedRadius);
        }
    }
}