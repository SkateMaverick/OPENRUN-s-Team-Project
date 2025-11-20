using UnityEngine;

namespace Player.Script
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerCombat : MonoBehaviour
    {
        public bool isAiming { get; private set; } = false;
        public Transform bowModel;

        private PlayerInput _playerInput;

        private void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            isAiming = _playerInput.AimInput;
        }
    }
}