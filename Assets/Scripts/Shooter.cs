using UnityEngine;

public abstract class Shooter : MonoBehaviour
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileSpeed;

    protected Rigidbody ProjectileRigidbody;

    protected virtual void Awake()
    {
        #region 초기화
        ProjectileRigidbody = GetComponent<Rigidbody>();
        #endregion
    }

    protected abstract void Shot();
}
