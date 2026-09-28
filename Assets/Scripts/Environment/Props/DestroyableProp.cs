using UnityEngine;

// Bullet에 맞았을 때, 파티클을 생성하고 자신 게임오브젝트를 파괴
public class DestroyableProp : MonoBehaviour, IDamageable
{
    [Header("Destroy Effect")]
    [SerializeField] private GameObject destroyParticlePrefab;
    [SerializeField] private Transform effectSpawnPoint;

    private bool _isDestroyed = false;

    public void TakeDamage(int damage)
    {
        if (!_isDestroyed)
        {
            _isDestroyed = true;
            SpawnDestroyParticle();
            Destroy(gameObject);
        }
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (_isDestroyed)
    //         return;
    //     
    //     // 충돌한 콜라이더가 투사체면 자신을 파괴
    //     if (other.TryGetComponent(out IProjectile projectile))
    //     {
    //         _isDestroyed = true;
    //         Destroyed();
    //     }
    // }
    //
    // private void Destroyed()
    // {
    //     SpawnDestroyParticle();
    //     Destroy(gameObject);
    // }

    private void SpawnDestroyParticle()
    {
        if (destroyParticlePrefab == null)
            return;

        Vector3 spawnPosition = effectSpawnPoint != null
            ? effectSpawnPoint.position
            : transform.position;

        Instantiate(
            destroyParticlePrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}