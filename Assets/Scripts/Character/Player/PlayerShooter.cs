using UnityEngine;
using Player.InputActions;

public class PlayerShooter : BaseShooter
{
    [SerializeField] private float fireDelay = 1f; // 다음 발사까지의 대기 시간
    private float _lastFireTime = 0f; // 마지막으로 발사한 시간을 기억할 변수
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        // 오른쪽 마우스 클릭이 들어오고 발사 가능 상태이며 발사 대기시간이 지났다면
        if (CanFire() && Time.time >= _lastFireTime + fireDelay)
        {
            // 투사체 발사
            Fire();
        }
    }

    private bool CanFire()
    {
        if (PlayerInputReader.Instance == null || !PlayerInputReader.Instance.FireInput)
        {
            return false;
        }

        // 큐(BoxGolem 또는 ScaleChange 컴포넌트를 가진 캐릭터)는 투사체를 발사하지 않음
        if (gameObject.name.Contains("Que") || GetComponent<ScaleChange>() != null)
        {
            return false;
        }

        // PlayerController가 존재할 때: 현재 큐를 플레이 중이거나 이 캐릭터가 활성 캐릭터가 아닌 경우 발사하지 않음
        if (PlayerController.Instance != null)
        {
            if (PlayerController.Instance.CurrentCharacterType == Enums.CharacterType.BoxGolem)
            {
                return false;
            }

            if (PlayerController.Instance.CurrentCharacterTransform != transform)
            {
                return false;
            }
        }

        return true;
    }

    protected override void Fire()
    {
        _lastFireTime = Time.time;

        if (projectilePrefab == null || projectileSpawnPoint == null)
            return;
        
        // 추후 변경
        if (Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation).TryGetComponent<IProjectile>(out IProjectile launchable))
        {
            launchable.Launch(projectileSpeed);
        }
    }
}
