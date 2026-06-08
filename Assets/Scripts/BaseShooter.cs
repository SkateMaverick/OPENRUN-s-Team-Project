using UnityEngine;

public abstract class BaseShooter : MonoBehaviour
{
    [SerializeField] protected GameObject projectilePrefab; // 발사체 프리팹
    [SerializeField] protected Transform projectileSpawnPoint; // 발사체 생성 위치
    [SerializeField] protected float projectileSpeed; // 발사체 속력

    protected Rigidbody ProjectileRigidbody;

    protected virtual void Awake()
    {
        #region 초기화
        ProjectileRigidbody = GetComponent<Rigidbody>();
        #endregion
    }

    protected abstract void Fire();
}
