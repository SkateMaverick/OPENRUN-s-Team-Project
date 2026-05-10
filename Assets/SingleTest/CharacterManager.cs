using UnityEngine;
using Enums;
using Player.InputActions;
using Player.Script;

public class CharacterManager : MonoBehaviour
{
    #region 싱글톤 선언
    public static CharacterManager Instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (_instance == null)
            {
                // 씬에서 CharacterChange 오브젝트를 찾아 할당
                _instance = FindAnyObjectByType<CharacterManager>();
            }

            // 싱글톤 오브젝트를 반환
            return _instance;
        }
    }
    #endregion

    private static CharacterManager _instance; // 싱글톤이 할당될 static 변수

    #region 현재 조작중인 캐릭터만 스크립트를 활성화시키기 위한 변수들
    private PlayerInputReader _sphereGolemInput;
    private PlayerInputReader _boxGolemInput;
    private LegacyPlayerMovement _sphereGolemMovement;
    private LegacyPlayerMovement _boxGolemMovement;
    #endregion
    private CharacterType _currentCharacter;

    private void Awake()
    {
        #region 싱글톤 처리
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (_instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
        #endregion
        
        // 기본으로 가지는 캐릭터는 큐
        _currentCharacter = CharacterType.SphereGolem;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바를 누르면 캐릭터 전환
        {
            _currentCharacter = _currentCharacter == CharacterType.SphereGolem ?  CharacterType.SphereGolem : CharacterType.BoxGolem;
        }
    }
}
