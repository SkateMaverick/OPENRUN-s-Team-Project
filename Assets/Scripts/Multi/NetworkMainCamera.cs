using Player.Script.BowPlayer;
using UnityEngine;
using Player.InputActions;

namespace Player.Script.CameraScript
{
    public class NetworkMainCamera : MonoBehaviour
    {
        [Header("Basic Settings")]
        public Transform defaultTarget;

        [Header("Distance Settings")]
        public float defaultDistance = 5.0f;
        public float aimDistance = 2.0f;

        [Header("Rotation Settings")]
        [Range(1f, 20f)] public float rotationSpeed = 3.0f;
        public float sensitivityMultiplier = 20f;
        public float topClamp = 70.0f;
        public float bottomClamp = -30.0f;

        [Header("Collision Settings")]
        public float cameraCollisionRadius = 0.2f;
        public LayerMask collisionLayers;

        // 외부에서 주입받을 변수들
        private NetworkPlayerInput _playerInput;
        private NetworkBowAimState _bowState;

        private float _currentYaw = 0f;
        private float _currentPitch = 0f;

        // ★ 핵심: NetworkManager가 이 함수를 호출해서 플레이어를 꽂아줍니다.
        public void SetTarget(GameObject player)
        {
            _playerInput = player.GetComponent<NetworkPlayerInput>();
            _bowState = player.GetComponent<NetworkBowAimState>();

            // CameraRoot 찾기
            defaultTarget = player.transform.Find("CameraRoot");
            if (defaultTarget == null) defaultTarget = player.transform; // 없으면 그냥 플레이어 자체를 봄

            // 커서 잠금 (게임 시작)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            // 플레이어가 아직 할당 안 됐으면 아무것도 안 함 (대기)
            if (_playerInput == null || defaultTarget == null) return;

            Vector2 lookInput = _playerInput.LookInput;
            _currentYaw += lookInput.x * rotationSpeed * Time.deltaTime * sensitivityMultiplier;
            _currentPitch -= lookInput.y * rotationSpeed * Time.deltaTime * sensitivityMultiplier;
            _currentPitch = Mathf.Clamp(_currentPitch, bottomClamp, topClamp);

            Quaternion cameraRotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
            Vector3 camDir = cameraRotation * Vector3.back;

            Transform normalRoot = defaultTarget;
            Transform aimRoot = defaultTarget;
            float lerpVal = 0f;

            if (_bowState != null)
            {
                if (_bowState.normalRoot != null) normalRoot = _bowState.normalRoot;
                if (_bowState.aimRoot != null) aimRoot = _bowState.aimRoot;
                lerpVal = _bowState.CurrentLerp;
            }
            else
            {
                if (defaultTarget == null) normalRoot = transform;
            }

            Vector3 posNormal = normalRoot.position + (camDir * defaultDistance);
            Vector3 posAim = aimRoot.position + (camDir * aimDistance);
            Vector3 idealPos = Vector3.Lerp(posNormal, posAim, lerpVal);
            Vector3 currentPivot = Vector3.Lerp(normalRoot.position, aimRoot.position, lerpVal);

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

            transform.position = finalPos;
            transform.rotation = cameraRotation;

            if (_bowState != null)
            {
                _bowState.RotateBow(_currentPitch);
            }
        }
    }
}