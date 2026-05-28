using Player.InputActions;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    public GameObject sphereGolem; // 큐 게임오브젝트
    public GameObject boxGolem; // 노아 게임오브젝트

    public CinemachineCamera sphereGolemVirtualCamera; // 큐를 타겟으로 하는 버추얼 카메라
    public CinemachineCamera boxGolemVirtualCamera; // 노아를 타겟으로 하는 버추얼 카메라

    private IControllable _sphereGolem;
    private IControllable _boxGolem;
    private IControllable _currentCharacter;

    private PlayerInputReader _playerInputReader;

    private void Awake()
    {
        _playerInputReader = GetComponent<PlayerInputReader>();

        _sphereGolem = sphereGolem.GetComponent<IControllable>();
        _boxGolem = boxGolem.GetComponent<IControllable>();

        // 기본 조종 캐릭터는 큐
        SwitchToSphereGolem();
    }

    private void Update()
    {
        // 키보드 상단 숫자 1번 = 노아
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToBoxGolem();
        }

        // 키보드 상단 숫자 2번 = 큐
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToSphereGolem();
        }
    }

    private void FixedUpdate()
    {
        if (_currentCharacter == null)
            return;

        _currentCharacter.HandleCharacterControl(
            _playerInputReader.MoveInput,
            _playerInputReader.SprintInput
        );

        Debug.Log($"MoveInput: {_playerInputReader.MoveInput}, Sprint: {_playerInputReader.SprintInput}");

        _currentCharacter.HandleCharacterControl(
            _playerInputReader.MoveInput,
            _playerInputReader.SprintInput
        );
    }

    private void SwitchToBoxGolem()
    {
        _currentCharacter = _boxGolem;

        if (sphereGolemVirtualCamera != null)
            sphereGolemVirtualCamera.gameObject.SetActive(false);

        if (boxGolemVirtualCamera != null)
            boxGolemVirtualCamera.gameObject.SetActive(true);
    }

    private void SwitchToSphereGolem()
    {
        _currentCharacter = _sphereGolem;

        if (sphereGolemVirtualCamera != null)
            sphereGolemVirtualCamera.gameObject.SetActive(true);

        if (boxGolemVirtualCamera != null)
            boxGolemVirtualCamera.gameObject.SetActive(false);
    }


}