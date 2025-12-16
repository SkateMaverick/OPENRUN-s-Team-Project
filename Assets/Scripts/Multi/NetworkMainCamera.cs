using UnityEngine;
using Player.InputActions;
// BowPlayer namespace 제거됨

// === 급하게 gemini 돌린 코드라서 이상할 수 있습니다. ===
namespace Player.Script.CameraScript
{
    public class NetworkMainCamera : MonoBehaviour
    {
        [Header("Basic Settings")]
        public Transform defaultTarget;

        [Header("Distance Settings")]
        public float defaultDistance = 5.0f;
        // aimDistance 제거됨 (활 관련)

        [Header("Rotation Settings")]
        [Range(1f, 20f)] public float rotationSpeed = 3.0f;
        public float sensitivityMultiplier = 20f;
        public float topClamp = 70.0f;
        public float bottomClamp = -30.0f;

        [Header("Collision Settings")]
        public float cameraCollisionRadius = 0.2f;
        public LayerMask collisionLayers;

        // BowState 제거됨
        private NetworkPlayerInput _playerInput;

        private float _currentYaw = 0f;
        private float _currentPitch = 0f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // 추가
            _playerInput = defaultTarget.GetComponent<NetworkPlayerInput>();
        }

        private void LateUpdate()
        {
            // 외부에서 _playerInput과 defaultTarget을 넣어준다고 가정하고 예외처리만 함
            if (_playerInput == null || defaultTarget == null) return;

            // 1. 입력 처리 및 회전 계산
            Vector2 lookInput = _playerInput.LookInput;
            _currentYaw += lookInput.x * rotationSpeed * Time.deltaTime * sensitivityMultiplier;
            _currentPitch -= lookInput.y * rotationSpeed * Time.deltaTime * sensitivityMultiplier;
            _currentPitch = Mathf.Clamp(_currentPitch, bottomClamp, topClamp);

            Quaternion cameraRotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
            Vector3 camDir = cameraRotation * Vector3.back;

            // 2. 타겟 위치 계산 (활 조준 로직 제거 -> 무조건 defaultTarget 기준)
            Vector3 currentPivot = defaultTarget.position;
            Vector3 idealPos = currentPivot + (camDir * defaultDistance);

            // 3. 충돌 처리 (벽 감지)
            Vector3 finalPos = idealPos;
            Vector3 dirToCamera = (idealPos - currentPivot).normalized;
            float distToCamera = Vector3.Distance(currentPivot, idealPos);

            RaycastHit[] hits = Physics.SphereCastAll(currentPivot, cameraCollisionRadius, dirToCamera, distToCamera, collisionLayers);
            float nearestHitDistance = distToCamera;
            bool foundWall = false;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Player")) continue;
                if (hit.distance < 0.01f) continue;

                if (hit.distance < nearestHitDistance)
                {
                    nearestHitDistance = hit.distance;
                    foundWall = true;
                }
            }

            if (foundWall)
            {
                if (nearestHitDistance <= 0.05f) nearestHitDistance = 0.05f;
                finalPos = currentPivot + (dirToCamera * nearestHitDistance);
            }

            // 4. 최종 적용
            transform.position = finalPos;
            transform.rotation = cameraRotation;
        }
    }
}