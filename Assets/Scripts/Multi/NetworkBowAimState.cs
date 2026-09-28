using UnityEngine;
using Player.InputActions;
using Photon.Pun;

namespace Player.Script.BowPlayer
{
    public class NetworkBowAimState : MonoBehaviourPun
    {
        [Header("Targets")]
        public Transform normalRoot;
        public Transform aimRoot;
        public Transform bowModel;

        [Header("Settings")]
        public float aimInSpeed = 10.0f;
        public float aimOutSpeed = 10.0f;

        [Header("Status")] public bool isAiming;
        
        public float CurrentLerp { get; private set; } 

        private NetworkPlayerInput _playerInput;

        private void Start()
        {
            _playerInput = GetComponent<NetworkPlayerInput>();
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            isAiming = _playerInput.AimInput;

            float targetValue = isAiming ? 1f : 0f;
            float speed = isAiming ? aimInSpeed : aimOutSpeed;

            // 0 ~ 1 사이의 진행률(Progress)만 계산해서 저장
            CurrentLerp = Mathf.MoveTowards(CurrentLerp, targetValue, speed * Time.deltaTime);
        }

        public void RotateBow(float pitch)
        {
            if (bowModel != null)
            {
                bowModel.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
        }
    }
}