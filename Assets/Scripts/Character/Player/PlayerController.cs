using Player.InputActions;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public event System.Action<Transform> OnCharacterChanged;

    public GameObject sphereGolem; // 노아 게임오브젝트
    public GameObject boxGolem; // 큐 게임오브젝트

    public CinemachineCamera sphereGolemVirtualCamera; // 노아를 타겟으로 하는 버추얼 카메라
    public CinemachineCamera boxGolemVirtualCamera; // 큐를 타겟으로 하는 버추얼 카메라

    private IControllable _sphereGolem;
    private IControllable _boxGolem;
    private IControllable _currentCharacter;

    private PlayerInputReader _playerInputReader;

    public Enums.CharacterType CurrentCharacterType => _currentCharacter == _boxGolem
        ? Enums.CharacterType.BoxGolem
        : Enums.CharacterType.SphereGolem;

    public Transform CurrentCharacterTransform => _currentCharacter == _boxGolem
        ? (boxGolem != null ? boxGolem.transform : null)
        : (sphereGolem != null ? sphereGolem.transform : null);

    private void Awake()
    {
        Instance = this;
        _playerInputReader = GetComponent<PlayerInputReader>();

        _sphereGolem = sphereGolem.GetComponent<IControllable>();
        _boxGolem = boxGolem.GetComponent<IControllable>();

        // 기본 조종 캐릭터는 노아
        SwitchToSphereGolem();
    }

    private void Update()
    {
        // 일시정지 중이거나 PausePanel이 열려있을 때는 캐릭터 변경 방지
        if (PauseUI.Instance != null && PauseUI.Instance.IsPaused)
        {
            return;
        }

        // 키보드 상단 숫자 1번 = 큐
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToBoxGolem();
        }

        // 키보드 상단 숫자 2번 = 노아
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
    }

    public void SwitchToBoxGolem()
    {
        if (_boxGolem == null && boxGolem != null) _boxGolem = boxGolem.GetComponent<IControllable>();
        _currentCharacter = _boxGolem;

        if (sphereGolemVirtualCamera != null)
            sphereGolemVirtualCamera.gameObject.SetActive(false);

        if (boxGolemVirtualCamera != null)
            boxGolemVirtualCamera.gameObject.SetActive(true);

        OnCharacterChanged?.Invoke(CurrentCharacterTransform);
    }

    public void SwitchToSphereGolem()
    {
        if (_sphereGolem == null && sphereGolem != null) _sphereGolem = sphereGolem.GetComponent<IControllable>();
        _currentCharacter = _sphereGolem;

        if (sphereGolemVirtualCamera != null)
            sphereGolemVirtualCamera.gameObject.SetActive(true);

        if (boxGolemVirtualCamera != null)
            boxGolemVirtualCamera.gameObject.SetActive(false);

        OnCharacterChanged?.Invoke(CurrentCharacterTransform);
    }


}