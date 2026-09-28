using System.Collections;
using UnityEngine;

public class SlimeSinglePlay : MonoBehaviour, IDamageable
{
    [Header("Status")]
    public int maxHealth = 20;
    public float speed = 1f;

    [Header("Target Search")]
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private string targetTag = "Player";

    [Header("Attack")]
    [SerializeField] private float attackRange = 2.2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackMotionTime = 0.9f;
    [SerializeField] private float attackHitDelay = 0.4f;
    [SerializeField] private string attackStateName01 = "Attack01";
    [SerializeField] private string attackStateName02 = "Attack02";
    [SerializeField] private bool randomAttackMotion = true;

    private int currentHealth;
    private Animator _animator;
    private Transform _target;
    private bool _isDead;
    private bool _isAttacking;
    private float _lastAttackTime;

    private bool HasTarget => _target != null && !_isDead;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (_isDead)
            return;

        if (_isAttacking)
            return;

        if (HasTarget)
        {
            float distance = Vector3.Distance(transform.position, _target.position);

            if (distance > attackRange)
            {
                MoveToTarget();
            }
            else
            {
                TryAttack();
            }
        }
        else
        {
            if (_animator != null)
                _animator.SetBool("HasTarget", false);
        }
    }

    private IEnumerator UpdatePath()
    {
        while (!_isDead)
        {
            FindTarget();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void FindTarget()
    {
        Collider[] colliders = new Collider[10];

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            detectRange,
            colliders,
            whatIsTarget
        );

        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider col = colliders[i];
            if (col == null)
                continue;

            if (!col.CompareTag(targetTag) && !col.transform.root.CompareTag(targetTag))
                continue;

            Transform targetTransform = col.transform.root != null ? col.transform.root : col.transform;
            if (targetTransform == transform)
                continue;

            float distance = Vector3.Distance(transform.position, targetTransform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = targetTransform;
            }
        }

        _target = closestTarget;

        if (_animator != null && !_isAttacking)
            _animator.SetBool("HasTarget", _target != null);
    }

    private void MoveToTarget()
    {
        Vector3 targetPosition = _target.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);

        transform.position += direction * speed * Time.deltaTime;

        if (_animator != null)
            _animator.SetBool("HasTarget", true);
    }

    private void TryAttack()
    {
        if (_animator != null)
            _animator.SetBool("HasTarget", false);

        LookAtTargetOnlyY();

        if (Time.time < _lastAttackTime + attackCooldown)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;
        _lastAttackTime = Time.time;

        LookAtTargetOnlyY();

        string attackState = attackStateName01;
        if (randomAttackMotion)
        {
            attackState = Random.value > 0.5f ? attackStateName01 : attackStateName02;
        }

        PlayAnimation(attackState);

        yield return new WaitForSeconds(attackHitDelay);

        if (_target != null && !_isDead)
        {
            float distance = Vector3.Distance(transform.position, _target.position);
            if (distance <= attackRange + 1.0f)
            {
                IDamageable damageable = _target.GetComponent<IDamageable>();
                if (damageable == null)
                {
                    damageable = _target.GetComponentInParent<IDamageable>();
                }

                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }

        float remainingTime = Mathf.Max(0f, attackMotionTime - attackHitDelay);
        yield return new WaitForSeconds(remainingTime);

        _isAttacking = false;
    }

    private void LookAtTargetOnlyY()
    {
        if (_target == null)
            return;

        Vector3 targetPosition = _target.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude <= 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    private void PlayAnimation(string stateName)
    {
        if (_animator == null || string.IsNullOrEmpty(stateName))
            return;

        int layerIndex = 0;
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(layerIndex);
        if (stateInfo.IsName(stateName))
            return;

        _animator.CrossFadeInFixedTime(stateName, 0.15f, layerIndex);
    }

    public void TakeDamage(int damage)
    {
        if (_isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_isDead)
            return;

        _isDead = true;

        if (_animator != null)
            _animator.SetTrigger("Die");

        if (Application.isPlaying)
            Destroy(gameObject, 5f);
        else
            DestroyImmediate(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}