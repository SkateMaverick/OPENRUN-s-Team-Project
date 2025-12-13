using System;
using UnityEngine;
using UnityEngine.InputSystem;
// 모바일
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Player.InputActions
{
    public class HandlePlayerInput : MonoBehaviour
    {
        private PlayerAction _playerAction;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool AimInput { get; private set; }
        public bool SprintInput { get; private set; }
        
        // 카메라 회전을 허용한 터치 ID를 저장하는 리스트
        private HashSet<int> _lookFingers = new HashSet<int>();

        private void Awake()
        {
            _playerAction = new PlayerAction();
        }

        private void OnEnable()
        {
            // Move (WASD)
            _playerAction.PlayerActions.Move.performed += OnMove;
            _playerAction.PlayerActions.Move.canceled += OnMove;

            // Look (Mouse Delta)
            _playerAction.PlayerActions.Look.performed += OnLook;
            _playerAction.PlayerActions.Look.canceled += OnLook;

            // Jump (Space)
            _playerAction.PlayerActions.Jump.started += OnJump;

            // Aim (Right Mouse Button)
            _playerAction.PlayerActions.Aim.performed += OnAim;
            _playerAction.PlayerActions.Aim.canceled += OnAim;

            // Sprint (Shift)
            _playerAction.PlayerActions.Sprint.performed += OnSprint;
            _playerAction.PlayerActions.Sprint.canceled += OnSprint;

            _playerAction.PlayerActions.Enable();
            
            // EnhancedTouch를 사용하기 위해 허용
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            _playerAction.PlayerActions.Move.performed -= OnMove;
            _playerAction.PlayerActions.Move.canceled -= OnMove;

            _playerAction.PlayerActions.Look.performed -= OnLook;
            _playerAction.PlayerActions.Look.canceled -= OnLook;

            _playerAction.PlayerActions.Jump.started -= OnJump;

            _playerAction.PlayerActions.Aim.performed -= OnAim;
            _playerAction.PlayerActions.Aim.canceled -= OnAim;

            _playerAction.PlayerActions.Sprint.performed -= OnSprint;
            _playerAction.PlayerActions.Sprint.canceled -= OnSprint;

            _playerAction.PlayerActions.Disable();
        }

        private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            JumpInput = true;
        }

        private void OnAim(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            AimInput = context.ReadValue<float>() > 0.5f;
        }

        private void OnSprint(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            SprintInput = context.ReadValue<float>() > 0.5f;
        }

        private void LateUpdate()
        {
            JumpInput = false;
        }

        #region 모바일 처리 (Update가 여기에 들어가있습니다)
        private void Update()
        {
            // 디바이스가 모바일일때만 HandleMobileLook 호출
            if (CurrentDevice.IsMobile)
            {
                HandleMobileLook();
            }
        }

        // 모바일에서 UI가 아닌 빈 곳을 터치했을 때는 드래그로 화면을 회전시킴
        private void HandleMobileLook()
        {
            // 매 프레임 초기화
            LookInput = Vector2.zero;
            
            // ETouch.activeTouches: 현재 터치중인 터치들의 수
            foreach (var touch in ETouch.activeTouches) 
            {
                // 현재 touch가 터치중인 손가락들 중 인덱스 번호가 무엇인지
                int fingerIndex = touch.finger.index;
                
                // 현재 touch의 단계가
                switch (touch.phase)
                {
                    // 터치가 시작된 순간일 때,
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        // 터치 포인터가 UI 위가 아니라면 화면 회전이 가능한 손가락으로 등록
                        if (!IsPointerOverUI(touch))
                        {
                            _lookFingers.Add(fingerIndex);
                        }
                        break;

                    // 터치가 움직이고 있다면, (드래그 중이라면)
                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        // 등록된 손가락이라면 LookInput에 델타값(얼마나 드래그했는지)을 넣어줌
                        if (_lookFingers.Contains(fingerIndex))
                        {
                            LookInput += touch.delta; 
                        }
                        break;

                    // 터치가 때진 순간일 때,
                    case UnityEngine.InputSystem.TouchPhase.Ended:
                    case UnityEngine.InputSystem.TouchPhase.Canceled:
                        // 화면 회전이 가능한 손가락들 리스트에서 등록을 해제
                        _lookFingers.Remove(fingerIndex);
                        break;
                }
            }
        }
        
        // 터치 포인터가 UI 위인지 판별
        private bool IsPointerOverUI(ETouch touch)
        {
            if (EventSystem.current == null) return false;
            
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = touch.screenPosition; 

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }
        #endregion
    }
}