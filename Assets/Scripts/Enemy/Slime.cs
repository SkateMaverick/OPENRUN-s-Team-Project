using System.Collections;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float speed = 1f;

    // 플레이어를 탐지할 범위
    [SerializeField] private float detectRange = 10f;

    // 이 거리 안으로 들어오면 더 이상 이동하지 않고 공격 모션을 재생
    [SerializeField] private float attackRange = 1.4f;

    // 탐지할 대상의 Layer
    [SerializeField] private LayerMask whatIsTarget;

    [Header("Attack Motion Only")]

    // 공격 모션을 너무 자주 반복하지 않도록 하는 쿨타임
    [SerializeField] private float attackCooldown = 1.5f;

    // 공격 애니메이션이 재생되는 동안 슬라임이 움직이지 않도록 기다리는 시간
    [SerializeField] private float attackMotionTime = 0.9f;

    // true이면 Attack01, Attack02 중 하나를 랜덤으로 재생
    [SerializeField] private bool randomAttackMotion = true;

    [Header("Animation State Names")]

    // Animator에 있는 State 이름과 정확히 같아야 함
    [SerializeField] private string idleStateName = "IdleBattle";
    [SerializeField] private string walkStateName = "WalkFWD";
    [SerializeField] private string attackStateName01 = "Attack01";
    [SerializeField] private string attackStateName02 = "Attack02";
    [SerializeField] private string dieStateName = "Die";

    private Animator animator;

    // 현재 추적 중인 대상
    // 싱글 플레이 기준이므로 Photon의 Health 대신 LivingEntity를 탐지 기준으로 사용
    private Transform target;

    private bool isDead;
    private bool isAttacking;

    private float lastAttackTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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

        // 이미 타겟 방향을 바라보므로 자신의 앞쪽 방향으로 이동
        transform.position += direction * speed * Time.deltaTime;

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

        /*
         * 현재는 데미지 처리 없이 공격 모션만 재생
         * 
         * 추후 체력 기능을 붙일 경우,
         * 이 코루틴 안에서 공격 타이밍에 맞춰
         * LivingEntity.ApplyDamage()를 호출
         */
        PlayAnimation(attackState);

        yield return new WaitForSeconds(attackMotionTime);

        isAttacking = false;
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

    /*
     * Die()는 외부에서 필요할 때 호출할 수 있도록만 남겨둠
     * 
     * 추후 LivingEntity와 연동한다면
     * 사망 처리 시 이 함수를 호출하거나,
     * LivingEntity의 사망 이벤트와 연결하면 됨
     */
    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        PlayAnimation(dieStateName);

        Destroy(gameObject, 5f);
    }

    /*
     * Scene 뷰에서 슬라임의 탐지 범위와 공격 범위를 확인하기 위한 기즈모
     * 노란색: 탐지 범위
     * 빨간색: 공격 범위
     */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}