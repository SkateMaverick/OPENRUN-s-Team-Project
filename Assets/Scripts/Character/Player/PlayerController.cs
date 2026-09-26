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

        if (sphereGolem == null)
            sphereGolem = GameObject.Find("Noa");
        if (boxGolem == null)
            boxGolem = GameObject.Find("Que");

        if (sphereGolem != null)
            _sphereGolem = sphereGolem.GetComponent<IControllable>();
        if (boxGolem != null)
            _boxGolem = boxGolem.GetComponent<IControllable>();

        InitializeCameras();

        // 기본 조종 캐릭터는 노아
        SwitchToSphereGolem();
    }

    private void InitializeCameras()
    {
        if (sphereGolemVirtualCamera == null)
        {
            var noaCamObj = GameObject.Find("NoaCam");
            if (noaCamObj != null)
                sphereGolemVirtualCamera = noaCamObj.GetComponent<CinemachineCamera>();

            if (sphereGolemVirtualCamera == null)
                sphereGolemVirtualCamera = FindFirstObjectByType<CinemachineCamera>();
        }

        if (boxGolemVirtualCamera == null)
        {
            var queCamObj = GameObject.Find("QueCam");
            if (queCamObj != null)
                boxGolemVirtualCamera = queCamObj.GetComponent<CinemachineCamera>();
        }
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

        if (boxGolemVirtualCamera != null && sphereGolemVirtualCamera != null && boxGolemVirtualCamera != sphereGolemVirtualCamera)
        {
            SyncCameraAxes(sphereGolemVirtualCamera, boxGolemVirtualCamera);
            sphereGolemVirtualCamera.gameObject.SetActive(false);
            boxGolemVirtualCamera.gameObject.SetActive(true);
        }
        else if (sphereGolemVirtualCamera != null)
        {
            if (boxGolem != null)
            {
                sphereGolemVirtualCamera.Target.TrackingTarget = boxGolem.transform;
                if (sphereGolemVirtualCamera.Target.LookAtTarget != null)
                    sphereGolemVirtualCamera.Target.LookAtTarget = boxGolem.transform;
            }
        }
        else if (boxGolemVirtualCamera != null)
        {
            if (boxGolem != null)
            {
                boxGolemVirtualCamera.Target.TrackingTarget = boxGolem.transform;
                if (boxGolemVirtualCamera.Target.LookAtTarget != null)
                    boxGolemVirtualCamera.Target.LookAtTarget = boxGolem.transform;
            }
        }

        OnCharacterChanged?.Invoke(CurrentCharacterTransform);
    }

    public void SwitchToSphereGolem()
    {
        if (_sphereGolem == null && sphereGolem != null) _sphereGolem = sphereGolem.GetComponent<IControllable>();
        _currentCharacter = _sphereGolem;

        if (boxGolemVirtualCamera != null && sphereGolemVirtualCamera != null && boxGolemVirtualCamera != sphereGolemVirtualCamera)
        {
            SyncCameraAxes(boxGolemVirtualCamera, sphereGolemVirtualCamera);
            boxGolemVirtualCamera.gameObject.SetActive(false);
            sphereGolemVirtualCamera.gameObject.SetActive(true);
        }
        else if (sphereGolemVirtualCamera != null)
        {
            if (sphereGolem != null)
            {
                sphereGolemVirtualCamera.Target.TrackingTarget = sphereGolem.transform;
                if (sphereGolemVirtualCamera.Target.LookAtTarget != null)
                    sphereGolemVirtualCamera.Target.LookAtTarget = sphereGolem.transform;
            }
        }
        else if (boxGolemVirtualCamera != null)
        {
            if (sphereGolem != null)
            {
                boxGolemVirtualCamera.Target.TrackingTarget = sphereGolem.transform;
                if (boxGolemVirtualCamera.Target.LookAtTarget != null)
                    boxGolemVirtualCamera.Target.LookAtTarget = sphereGolem.transform;
            }
        }

        OnCharacterChanged?.Invoke(CurrentCharacterTransform);
    }

    private void SyncCameraAxes(CinemachineCamera fromCam, CinemachineCamera toCam)
    {
        if (fromCam == null || toCam == null) return;
        var fromOrbital = fromCam.GetComponent<CinemachineOrbitalFollow>();
        var toOrbital = toCam.GetComponent<CinemachineOrbitalFollow>();
        if (fromOrbital != null && toOrbital != null)
        {
            toOrbital.HorizontalAxis.Value = fromOrbital.HorizontalAxis.Value;
            toOrbital.VerticalAxis.Value = fromOrbital.VerticalAxis.Value;
            toOrbital.RadialAxis.Value = fromOrbital.RadialAxis.Value;
        }
    }


}