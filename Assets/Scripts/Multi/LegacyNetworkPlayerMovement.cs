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
        public float groundCheckDistance = 0.2f;

        private Rigidbody _rb;
        private BoxCollider _boxCol; // Capsule 대신 BoxCollider로 수정
        private Transform _mainCameraTransform;
        private NetworkPlayerInput _playerInput;

        private float _inputH;
        private float _inputV;
        private bool _isSprinting;
        private float _camEulerY;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _boxCol = GetComponent<BoxCollider>(); // 컴포넌트 할당 수정

            _rb.freezeRotation = true;
            _rb.useGravity = false;

            if (Camera.main != null)
            {
                _mainCameraTransform = Camera.main.transform;
            }

            _playerInput = GetComponent<NetworkPlayerInput>();
        }

        private void Update()
        {
            if (!photonView.IsMine) return;

            if (_playerInput != null)
            {
                _inputH = _playerInput.MoveInput.x;
                _inputV = _playerInput.MoveInput.y;
                _isSprinting = _playerInput.SprintInput && !_playerInput.AimInput;
            }

            if (_mainCameraTransform != null)
            {
                _camEulerY = _mainCameraTransform.eulerAngles.y;
            }
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine) return;

            GroundedCheck();
            Gravity();
            Move();
        }

        private void Move()
        {
            Vector3 inputVector = new Vector3(_inputH, 0f, _inputV).normalized;
            Quaternion cameraRotation = Quaternion.Euler(0, _camEulerY, 0);

            _lastMoveDirection = cameraRotation * inputVector;
            
            if (GetGroundHit(_rb.position + Vector3.up * 0.1f, Vector3.down, 1.0f, out RaycastHit hitInfo))
            {
                if (grounded)
                {
                    _lastMoveDirection = Vector3.ProjectOnPlane(_lastMoveDirection, hitInfo.normal).normalized;
                }
            }

            float finalHorizontalSpeed = 0f;

            if (_inputH != 0 || _inputV != 0)
            {
                finalHorizontalSpeed = _isSprinting ? moveSpeed * 2.0f : moveSpeed;
            }
            
            Vector3 finalVelocity = (_lastMoveDirection * finalHorizontalSpeed) + (Vector3.up * currentVelocity);
            Vector3 nextPosition = _rb.position + (finalVelocity * Time.fixedDeltaTime);
            
            Quaternion targetRotation = Quaternion.Euler(0, _camEulerY, 0);

            _rb.Move(nextPosition, targetRotation);
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
                    currentVelocity += gravity * Time.fixedDeltaTime;
                }
            }
        }
        
        private void GroundedCheck()
        {
            if (_boxCol == null) return;

            // BoxCollider의 크기를 기반으로 검사 시작점과 범위를 설정합니다.
            Vector3 origin = _boxCol.bounds.center;
            // 박스의 절반 높이(extents.y)에 추가 거리를 더해 검사 거리를 설정합니다.
            float distance = _boxCol.bounds.extents.y + groundCheckDistance;
            
            grounded = GetGroundHit(origin, Vector3.down, distance, out _);
        }

        private bool GetGroundHit(Vector3 origin, Vector3 dir, float maxDist, out RaycastHit closestHit)
        {
            closestHit = new RaycastHit();
            bool hitFound = false;
            float minDistance = maxDist;

            // BoxCollider의 가로(x), 세로(z) 크기보다 아주 미세하게 작은 직육면체(Box) 볼륨을 만듭니다.
            // 0.95f를 곱하는 이유는 박스 벽면과 마찰로 인한 오작동을 방지하기 위함입니다.
            Vector3 boxHalfExtents = new Vector3(_boxCol.bounds.extents.x * 0.95f, 0.05f, _boxCol.bounds.extents.z * 0.95f);
            
            // SphereCast 대신 BoxCast를 사용하여 박스 형태의 범위로 바닥 충돌 검사
            RaycastHit[] hits = Physics.BoxCastAll(origin, boxHalfExtents, dir, transform.rotation, maxDist, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.root == transform.root) continue;

                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    closestHit = hit;
                    hitFound = true;
                }
            }

            return hitFound;
        }
    }
}