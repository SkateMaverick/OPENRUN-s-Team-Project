using UnityEngine;
using UnityEngine.Serialization;

public class Dispenser : MonoBehaviour
{
    // 디스펜서가 발사할 투사체
    [SerializeField] private GameObject projectilePrefab;
    // 투사체가 생성될 위치
    [SerializeField] private Transform projectileSpawnPoint;
    // 투사체가 날아가는 속도
    [SerializeField] private float projectileSpeed = 20;
    // 디스펜서가 투사체를 쏘는 간격
    [SerializeField] private float fireRate = 0.75f;
    
    [SerializeField] private AudioClip shotAClip;
    
    // 현재 경과 시간
    private float _currentRate = 0f;
    
    private void Update()
    {
        _currentRate += Time.deltaTime;
        
        if (_currentRate >= fireRate)
        {
            Fire();
    
            _currentRate = 0f;
        }
    }
    
    private void Fire()
    {
        if (Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation).TryGetComponent<IProjectile>(out IProjectile launchable))
        {
            launchable.Launch(projectileSpeed);
        }
        
        // shotAudio가 할당되어 있는 디스펜서만 오디오 출력
        if (shotAClip != null)
        {
            AudioManager.Instance.PlaySFX(shotAClip, 0.5f);
        }
    }
}
