using UnityEngine;

namespace Player.Script
{
    public class MainCamera : MonoBehaviour
    {
        public Transform target;
        public Transform aimTarget;
        public float aimTransitionSpeed = 20f;
        
        private PlayerInput _playerInput;
        private PlayerCombat _playerCombat;
        private Vector3 _currentTargetPosition;

        public float distance = 5.0f;
        public float height = 0.0f;
        public float aimDistance = 3.0f;
        private float _currentDistance;

        [Range(1f, 20f)] public float rotationSpeed = 5.0f;
        public float sensitivityMultiplier = 100f;

        private float _currentYaw = 0f;
        private float _currentPitch = 0f;

        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;

        public float minDistance = 0.2f;
        public float cameraCollisionRadius = 0.2f;
        public LayerMask collisionLayers;

        private void Start()
        {
            if (target == null)
            {
                TryFindPlayerTarget();
            }

            if (target != null)
            {
                _currentPitch = -Mathf.Atan2(height, distance) * Mathf.Rad2Deg;
                _currentYaw = target.eulerAngles.y;
                _currentTargetPosition = target.position;
                _currentDistance = distance;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (!HandleTargeting()) return;

            HandleRotation();
            
            HandlePivotAndDistance();

            HandlePositioningAndCollision();

            HandleBowRotation();
        }
        
        private bool HandleTargeting()
        {
            if (target != null && _playerCombat != null && _playerInput != null)
            {
                return true;
            }
            
            TryFindPlayerTarget();
            return (target != null && _playerCombat != null && _playerInput != null);
        }

        private void HandleRotation()
        {
            Vector2 lookInput = _playerInput.LookInput;
            
            _currentYaw += lookInput.x * rotationSpeed * Time.deltaTime * sensitivityMultiplier; 
            _currentPitch -= lookInput.y * rotationSpeed * Time.deltaTime * sensitivityMultiplier; 
            
            _currentPitch = Mathf.Clamp(_currentPitch, BottomClamp, TopClamp);
        }

        private void HandlePivotAndDistance()
        {
            bool isAiming = _playerCombat.isAiming && aimTarget != null;
            
            Vector3 targetPivot = isAiming ? aimTarget.position : target.position;
            _currentTargetPosition = Vector3.Lerp(_currentTargetPosition, targetPivot, 
                                                  aimTransitionSpeed * Time.deltaTime);
            
            float targetDistance = isAiming ? aimDistance : distance;
            _currentDistance = Mathf.Lerp(_currentDistance, targetDistance, 
                                          aimTransitionSpeed * Time.deltaTime);
        }

        private void HandlePositioningAndCollision()
        {
            Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
            Vector3 targetPosition = _currentTargetPosition + rotation * new Vector3(0f, height, -_currentDistance);

            transform.position = targetPosition;
            transform.LookAt(_currentTargetPosition);

            CameraCollisionCheck(_currentTargetPosition);
        }

        private void HandleBowRotation()
        {
            if (_playerCombat != null && _playerCombat.bowModel != null)
            {
                _playerCombat.bowModel.localRotation = Quaternion.Euler(_currentPitch, 0f, 0f);
            }
        }

        private void TryFindPlayerTarget()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                _playerCombat = playerObj.GetComponent<PlayerCombat>();
                _playerInput = playerObj.GetComponent<PlayerInput>(); 

                if (target == null)
                    target = playerObj.transform.Find("CameraRoot"); 
                if (aimTarget == null)
                    aimTarget = playerObj.transform.Find("AimingRoot");
                
                if (target != null)
                {
                    _currentTargetPosition = target.position;
                    _currentDistance = distance;
                }
            }
        }

        private void CameraCollisionCheck(Vector3 currentPivot)
        {
            Vector3 desiredPosition = transform.position;
            Vector3 direction = (desiredPosition - currentPivot).normalized;
            float currentDistance = Vector3.Distance(desiredPosition, currentPivot);

            RaycastHit hit;

            if (Physics.SphereCast(currentPivot, cameraCollisionRadius, direction, out hit, currentDistance,
                    collisionLayers))
            {
                transform.position = hit.point + hit.normal * cameraCollisionRadius;
            }
        }
    }
}