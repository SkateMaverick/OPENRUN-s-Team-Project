using UnityEngine;
using Player.InputActions;

namespace Player.Script
{
    public class LegacyPlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 4.0f;
        private Vector3 _lastMoveDirection;

        [Header("Ground Settings")]
        public bool grounded = true;
        public float groundCheckDistance = 0.2f;

        private Rigidbody _rb;
        private Collider _col;
        private Transform _mainCameraTransform;
        public PlayerInputReader _playerInputReader; 

        private float _inputH;
        private float _inputV;
        private bool _isSprinting;
        private float _camEulerY;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _col = GetComponent<Collider>(); 

            // 물리 엔진 회전 제어 및 자체 중력 활성화
            _rb.freezeRotation = true;
            _rb.useGravity = true; 

            if (Camera.main != null)
            {
                _mainCameraTransform = Camera.main.transform;
            }

            _playerInputReader = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            // 이동 및 카메라 입력값 캐싱
            if (_playerInputReader != null)
            {
                _inputH = _playerInputReader.MoveInput.x;
                _inputV = _playerInputReader.MoveInput.y;
                //_isSprinting = _playerInputReader.SprintInput && !_playerInputReader.AimInput;
            }
        
            if (_mainCameraTransform != null)
            {
                _camEulerY = _mainCameraTransform.eulerAngles.y;
            }
        }

        private void FixedUpdate()
        {
            GroundedCheck();
            //Move();
        }

        // 추후 확인 필요
        public void Move()
        {
            Vector3 inputVector = new Vector3(_inputH, 0f, _inputV).normalized;
            Quaternion cameraRotation = Quaternion.Euler(0, _camEulerY, 0);

            _lastMoveDirection = cameraRotation * inputVector;
    
            // 경사면 이동 방향 보정
            if (GetGroundHit(_col.bounds.center, Vector3.down, _col.bounds.extents.y + 1.0f, out RaycastHit hitInfo))
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
    
            // 수평 이동 벡터 계산 (경사면일 경우 y축 이동 방향도 포함되어 있음)
            Vector3 finalVelocity = _lastMoveDirection * finalHorizontalSpeed;
    
            // 땅에 닿아있으면 경사면 방향(y축 포함)대로 이동하고, 공중이면 중력 영향 유지
            if (grounded)
            {
                _rb.linearVelocity = finalVelocity;
            }
            else
            {
                _rb.linearVelocity = new Vector3(finalVelocity.x, _rb.linearVelocity.y, finalVelocity.z);
            }

            Quaternion targetRotation = Quaternion.Euler(0, _camEulerY, 0);
            _rb.MoveRotation(targetRotation);
        }

        private void GroundedCheck()
        {
            // 콜라이더 중앙을 기준으로 바닥 체크 수행
            Vector3 origin = _col.bounds.center;
            float distance = _col.bounds.extents.y + groundCheckDistance;
            grounded = GetGroundHit(origin, Vector3.down, distance, out _);
        }

        private bool GetGroundHit(Vector3 origin, Vector3 dir, float maxDist, out RaycastHit closestHit)
        {
            closestHit = new RaycastHit();
            bool hitFound = false;
            float minDistance = maxDist;

            RaycastHit[] hits = Physics.RaycastAll(origin, dir, maxDist, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            
            foreach (RaycastHit hit in hits)
            {
                // 플레이어 자신(루트 오브젝트)과의 충돌 무시
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