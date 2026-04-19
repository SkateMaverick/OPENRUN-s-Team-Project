using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class EnemyTurretController : MonoBehaviourPun
{
    [Header("Target Search")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float targetRefreshInterval = 0.25f;

    [Header("Rotation")]
    [SerializeField] private Transform headPivot;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Attack")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float attackAngle = 8f;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 20f;

    [Header("Line Of Sight")]
    [SerializeField] private LayerMask obstacleMask;

    private TurretTargetable _currentTarget;
    private float _lastSearchTime;
    private float _lastFireTime;

    private void Update()
    {
        // 네트워크 환경에서는 MasterClient만 AI 수행
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
            return;

        UpdateTarget();
        RotateToTarget();
        TryAttack();
    }

    private void UpdateTarget()
    {
        if (Time.time < _lastSearchTime + targetRefreshInterval)
            return;

        _lastSearchTime = Time.time;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        TurretTargetable bestTarget = null;
        float bestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            TurretTargetable target = hit.GetComponentInParent<TurretTargetable>();
            if (target == null)
                continue;

            Health hp = target.GetComponent<Health>();
            if (hp == null || hp.CurrentHealth <= 0f)
                continue;

            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = target;
            }
        }

        _currentTarget = bestTarget;
    }

    private void RotateToTarget()
    {
        if (_currentTarget == null || headPivot == null)
            return;

        Vector3 targetPos = _currentTarget.AimPoint.position;
        Vector3 dir = targetPos - headPivot.position;

        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        headPivot.rotation = Quaternion.Slerp(
            headPivot.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    private void TryAttack()
    {
        if (_currentTarget == null || muzzlePoint == null)
            return;

        Vector3 targetPos = _currentTarget.AimPoint.position;
        Vector3 toTarget = targetPos - muzzlePoint.position;
        float distance = toTarget.magnitude;

        if (distance > attackRange)
            return;

        Vector3 dir = toTarget.normalized;
        float angle = Vector3.Angle(muzzlePoint.forward, dir);

        if (angle > attackAngle)
            return;

        if (!HasLineOfSight(targetPos, distance))
            return;

        if (Time.time < _lastFireTime + fireInterval)
            return;

        _lastFireTime = Time.time;

        Fire(dir, distance);
    }

    private bool HasLineOfSight(Vector3 targetPos, float distance)
    {
        Vector3 origin = muzzlePoint.position;
        Vector3 dir = (targetPos - origin).normalized;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, ~0))
        {
            TurretTargetable target = hit.collider.GetComponentInParent<TurretTargetable>();
            if (target != null && target == _currentTarget)
                return true;

            if (((1 << hit.collider.gameObject.layer) & obstacleMask) != 0)
                return false;
        }

        return true;
    }

    private void Fire(Vector3 direction, float distance)
    {
        // 가장 단순하고 안정적인 방식: 히트스캔
        if (Physics.Raycast(muzzlePoint.position, direction, out RaycastHit hit, attackRange, ~0))
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (muzzlePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(muzzlePoint.position, muzzlePoint.forward * attackRange);
        }
    }
}