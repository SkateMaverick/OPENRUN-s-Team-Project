using System.Collections;
using UnityEngine;

public class Slime : MonoBehaviour, IDamageable
{
    [Header("Status")]
    [SerializeField] private int maxHealth = 30;

    [Header("Move")]
    [SerializeField] private float speed = 2.5f;

    // 플레이어를 탐지할 범위
    [SerializeField] private float detectRange = 15f;

    // 이 거리 안으로 들어오면 더 이상 이동하지 않고 공격 모션을 재생
    [SerializeField] private float attackRange = 2.0f;

    // 탐지할 대상의 Layer
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float attackHitDelay = 0.4f;

    // 공격 모션을 너무 자주 반복하지 않도록 하는 쿨타임
    [SerializeField] private float attackCooldown = 1.5f;

    // 공격 애니메이션이 재생되는 동안 슬라임이 움직이지 않도록 기다리는 시간
    [SerializeField] private float attackMotionTime = 0.9f;

    // true이면 Attack01, Attack02 중 하나를 랜덤으로 재생
    [SerializeField] private bool randomAttackMotion = true;

    [Header("Audio Feedback")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;

    [Header("Drop Settings")]
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] [Range(0f, 1f)] private float dropRate = 0.75f;

    [Header("Animation State Names")]
    // Animator에 있는 State 이름과 정확히 같아야 함
    [SerializeField] private string idleStateName = "IdleBattle";
    [SerializeField] private string walkStateName = "WalkFWD";
    [SerializeField] private string attackStateName01 = "Attack01";
    [SerializeField] private string attackStateName02 = "Attack02";
    [SerializeField] private string dieStateName = "Die";

    private Animator animator;

    // 현재 추적 중인 대상
    private Transform target;

    private int currentHealth;
    private bool isDead;
    private bool isAttacking;

    private float lastAttackTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (whatIsTarget.value == 0)
        {
            whatIsTarget = LayerMask.GetMask("Player");
        }
    }

    private void Start()
    {
        StartCoroutine(UpdateTarget());
    }

    private void Update()
    {
        if (isDead)
            return;

        // 공격 모션이 재생되는 동안에는 이동하지 않음
        if (isAttacking)
            return;

        // 타겟이 없으면 대기 애니메이션을 재생합니다.
        if (target == null)
        {
            PlayAnimation(idleStateName);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        // 타겟이 공격 범위 밖에 있으면 추적
        if (distance > attackRange)
        {
            MoveToTarget();
        }
        // 타겟이 공격 범위 안에 있으면 이동을 멈추고 공격 모션을 재생
        else
        {
            TryAttackMotion();
        }
    }

    private IEnumerator UpdateTarget()
    {
        while (!isDead)
        {
            FindTarget();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void FindTarget()
    {
        // 1. 현재 조종 중인 활성 캐릭터 우선 탐지
        if (PlayerController.Instance != null && PlayerController.Instance.CurrentCharacterTransform != null)
        {
            Transform activeChar = PlayerController.Instance.CurrentCharacterTransform;
            float dist = Vector3.Distance(transform.position, activeChar.position);
            if (dist <= detectRange)
            {
                target = activeChar;
                return;
            }
        }

        // 2. 주변 콜라이더 기반 탐색
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
            if (colliders[i] == null)
                continue;
            LivingEntity livingEntity = colliders[i].GetComponentInParent<LivingEntity>();

            if (livingEntity == null)
                continue;

            // 자기 자신을 타겟으로 잡는 상황 방지
            if (livingEntity.transform == transform)
                continue;

            float distance = Vector3.Distance(transform.position, livingEntity.transform.position);

            // 가장 가까운 LivingEntity를 타겟으로 설정
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = livingEntity.transform;
            }
        }

        target = closestTarget;
    }

    private void MoveToTarget()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        // 타겟 방향을 바라봄
        transform.rotation = Quaternion.LookRotation(direction);

        // 이동 및 지형 스냅
        Vector3 nextPos = transform.position + direction * speed * Time.deltaTime;

        if (Physics.Raycast(nextPos + Vector3.up * 1.5f, Vector3.down, out RaycastHit hit, 5f, ~LayerMask.GetMask("Player", "Damageable", "Ignore Raycast")))
        {
            nextPos.y = hit.point.y;
        }

        transform.position = nextPos;

        // 이동 중에는 걷기 애니메이션 재생
        PlayAnimation(walkStateName);
    }

    private void TryAttackMotion()
    {
        // 공격 쿨타임이 아직 끝나지 않았으면 대기 상태 유지
        if (Time.time < lastAttackTime + attackCooldown)
        {
            PlayAnimation(idleStateName);
            return;
        }

        StartCoroutine(AttackMotionRoutine());
    }

    private IEnumerator AttackMotionRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // 공격하기 전에 타겟을 바라보게 함
        LookAtTargetOnlyY();

        string attackState = attackStateName01;

        // Attack01, Attack02 중 랜덤으로 하나 재생
        if (randomAttackMotion)
        {
            attackState = Random.value > 0.5f ? attackStateName01 : attackStateName02;
        }

        PlayAnimation(attackState);

        if (attackSound != null)
        {
            AudioSource.PlayClipAtPoint(attackSound, transform.position);
        }

        yield return new WaitForSeconds(attackHitDelay);

        if (target != null && !isDead)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= attackRange + 1.2f)
            {
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable == null)
                {
                    damageable = target.GetComponentInParent<IDamageable>();
                }

                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage);
                }
            }
        }

        float remainingTime = Mathf.Max(0f, attackMotionTime - attackHitDelay);
        yield return new WaitForSeconds(remainingTime);

        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void LookAtTargetOnlyY()
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    private void PlayAnimation(string stateName)
    {
        if (animator == null)
            return;

        if (string.IsNullOrEmpty(stateName))
            return;

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"{gameObject.name}에 Animator Controller가 없습니다.");
            return;
        }

        int layerIndex = 0;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (stateInfo.IsName(stateName))
            return;

        animator.CrossFadeInFixedTime(stateName, 0.15f, layerIndex);
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // 충돌체 비활성화하여 플레이어가 통과할 수 있도록 함
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // 아이템 드랍
        if (dropPrefab != null && Random.value <= dropRate)
        {
            Instantiate(dropPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        PlayAnimation(dieStateName);

        if (Application.isPlaying)
            Destroy(gameObject, 3f);
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