using UnityEngine;

namespace Player.Script
{
    public class ThirdPersonController : MonoBehaviour
    {
        #region Movement Variables
        public float MoveSpeed = 4.0f;
        public float SpeedChangeRate = 15.0f;
        public float gravity = -20f;
        [HideInInspector] public float currentSpeed;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _targetSpeed;
        private Vector3 _lastMoveDirection;
        #endregion

        #region Jump Variables
        public float JumpHeight = 1.2f;
        private float _fallTimeoutDelta;
        public float FallTimeout = 0.15f;
        [HideInInspector] public bool isJumping = false;
        [HideInInspector] public bool isFalling = false;
        #endregion

        #region Ground Variables
        public bool Grounded = true;
        public float GroundedOffset = 0.1f;
        public float GroundedRadius = 0.2f;
        public LayerMask GroundLayers;
        #endregion

        #region Player Variables
        private CharacterController _controller;
        private Animator _animator;
        private Transform _mainCameraTransform;
        private PlayerInput _playerInput;
        #endregion

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _mainCameraTransform = Camera.main.transform;

            _playerInput = GetComponent<PlayerInput>();

            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            GroundedCheck();
            Jump();
            Gravity();
            Move();
        }

        private void Move()
        {
            float horizontalInput = _playerInput.MoveInput.x;
            float verticalInput = _playerInput.MoveInput.y;

            Vector3 cameraForward = new Vector3(_mainCameraTransform.forward.x, 0, _mainCameraTransform.forward.z).normalized;
            Vector3 cameraRight = new Vector3(_mainCameraTransform.right.x, 0, _mainCameraTransform.right.z).normalized;

            _lastMoveDirection = cameraForward * verticalInput + cameraRight * horizontalInput;
            
            Quaternion targetRotation = Quaternion.Euler(0, _mainCameraTransform.eulerAngles.y, 0);
            transform.rotation = targetRotation;

            // Sprint 로직 추가
            float targetSpeed = MoveSpeed;
            bool isSprinting = _playerInput.SprintInput && !_playerInput.AimInput;
            targetSpeed = isSprinting ? MoveSpeed * 2.0f : MoveSpeed;

            if (horizontalInput != 0 || verticalInput != 0)
            {
                _targetSpeed = targetSpeed;
                _lastMoveDirection.Normalize();
            }
            else
            {
                _targetSpeed = 0f;
            }
    
            currentSpeed = Mathf.Lerp(currentSpeed, _targetSpeed, SpeedChangeRate * Time.deltaTime);

            Vector3 finalVelocity = (_lastMoveDirection * currentSpeed) + (Vector3.up * _verticalVelocity);
            _controller.Move(finalVelocity * Time.deltaTime);
        }

        private void Jump()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_playerInput.JumpInput)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * gravity);
                    isJumping = true;
                }
            }
            else
            {
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    isFalling = true;
                }
            }
        }

        private void Gravity()
        {
            if (Grounded)
            {
                if (isFalling)
                {
                    _verticalVelocity = 0f;
                    isJumping = false;
                    isFalling = false;
                }
                else if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }
            else
            {
                if (_verticalVelocity < 0.0f)
                {
                    isFalling = true;
                }

                if (_verticalVelocity < _terminalVelocity)
                {
                    _verticalVelocity += gravity * Time.deltaTime;
                }
            }
        }
        
        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + GroundedOffset,
                transform.position.z);

            Gizmos.DrawWireSphere(spherePosition, GroundedRadius);
        }
    }
}