using System.Collections;
using UnityEngine;

public class SlimeSinglePlay : MonoBehaviour
{
    [Header("Status")]
    public int maxHealth = 1;
    public float speed = 1f;

    [Header("Target Search")]
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private string targetTag = "Player";

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.2f;

    private int currentHealth;
    private Animator _animator;
    private Transform _target;
    private bool _isDead;

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

        if (HasTarget)
        {
            float distance = Vector3.Distance(transform.position, _target.position);

            if (distance > attackRange)
            {
                MoveToTarget();
            }
            else
            {
                StopAndAttack();
            }
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

            if (!col.CompareTag(targetTag))
                continue;

            float distance = Vector3.Distance(transform.position, col.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = col.transform;
            }
        }

        _target = closestTarget;

        if (_animator != null)
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

    private void StopAndAttack()
    {
        if (_animator != null)
        {
            _animator.SetBool("HasTarget", false);
            // 공격 애니메이션이 있으면 아래 Trigger 사용 가능
            // _animator.SetTrigger("Attack");
        }
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

        Destroy(gameObject, 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}