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
        // 오른쪽 마우스 클릭이 들어오고 있고, 현재 시간이 마지막 발사 시각 + 발사 대기시간보다 >=라면
        if (PlayerInputReader.Instance.FireInput && Time.time >= _lastFireTime + fireDelay)
        {
            // 투사체 발사
            Fire();
        }
    }

    protected override void Fire()
    {
        _lastFireTime = Time.time;
        
        // 추후 변경
        if (Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation).TryGetComponent<IProjectile>(out IProjectile launchable))
        {
            launchable.Launch(projectileSpeed);
        }
    }
}
