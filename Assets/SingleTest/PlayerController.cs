using Player.InputActions;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    public GameObject sphereGolem; // 큐 게임오브젝트
    public GameObject boxGolem; // 노아 게임오브젝트
    public CinemachineCamera sphereGolemVirtualCamera; // 큐를 타겟으로 버추얼 카메라
    public CinemachineCamera boxGolemVirtualCamera; // 노아를 타겟으로 하는 버추얼 카메라

    private IControllable _sphereGolem; // HandleCharacterControl를 실행하기 위해
    private IControllable _boxGolem; // ..
    private IControllable _currentCharacter; // 현재 조종 캐릭터
    private PlayerInputReader _playerInputReader;
    
    private void Awake()
    {
        #region 초기화
        _playerInputReader = GetComponent<PlayerInputReader>();
        _sphereGolem =  sphereGolem.GetComponent<IControllable>();
        _boxGolem = boxGolem.GetComponent<IControllable>();
        #endregion
    
        // 처음으로 가지는 기본 캐릭터는 스피어 골렘
        _currentCharacter = _sphereGolem;
    }
    
    private void Update()
    {
        // 일단은 Input Manager 방식
        // 스페이스바를 누르면 캐릭터 전환
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 현재 조종중인 캐릭터가 스피어 골렘이면 박스골렘으로, 박스 골렘이면 스피어 골렘으로 변경
            _currentCharacter = _currentCharacter == _sphereGolem ? _boxGolem : _sphereGolem;
            if (_currentCharacter == _sphereGolem)
            {
                sphereGolemVirtualCamera.gameObject.SetActive(true);
                boxGolemVirtualCamera.gameObject.SetActive(false);
            }
            else // _currentCharacter == _boxGolem
            {
                sphereGolemVirtualCamera.gameObject.SetActive(false);
                boxGolemVirtualCamera.gameObject.SetActive(true);
            }
        }
    }
    
    private void FixedUpdate()
    {
        // 현재 조종중인 캐릭터에게만 움직임과 회전
        _currentCharacter.HandleCharacterControl(_playerInputReader.MoveInput, _playerInputReader.SprintInput);
    }
}
