using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Slime : Health
{
    public float speed = 1f;
    
    [SerializeField] private LayerMask whatIsTarget;

    private Animator _animator;
    private Health _targetEntity;
    private bool _isDead;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 마스터 클라이언트에서만 AI 구동
        if (!PhotonNetwork.IsMasterClient) return;
        
        StartCoroutine(UpdatePath());
    }
    
    private bool hasTarget
    {
        get
        {
            // 타겟이 존재하고, "살아있을 때(!)"만 true 반환
            return _targetEntity != null && !_targetEntity.isDead;
        }
    }

    // 💡 이동은 부드럽게 보여야 하므로 Update()에서 매 프레임 실행합니다.
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient || _isDead) return;

        if (hasTarget)
        {
            // 타겟을 바라봄
            transform.LookAt(_targetEntity.transform.position);
            
            // 이미 LookAt으로 앞을 바라보게 했으므로, 그냥 자신의 앞쪽(forward)으로 전진하면 됩니다.
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    // 무거운 물리 연산(탐색)은 코루틴을 통해 0.25초마다 실행하여 최적화합니다.
    private IEnumerator UpdatePath()
    {
        while (!_isDead)
        {
            if (!hasTarget)
            {
                Collider[] colliders = new Collider[5];
                int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 10f, colliders, whatIsTarget);
                
                // 에러 해결: 0 이상(>= 0)이 아니라 0 초과(> 0)일 때만 체크하도록 변경!
                if (numColliders > 0)
                { 
                    // 안전하게 Health 컴포넌트를 가져오는지 확인
                    if (colliders[0].TryGetComponent<Health>(out var health))
                    {
                        _targetEntity = health;
                        _animator.SetBool("HasTarget", true);
                    }
                }
                else
                {
                    // 타겟을 못 찾았으면 애니메이션 멈춤
                    _animator.SetBool("HasTarget", false);
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    protected override void Die()
    {
        base.Die();
        _isDead = true; // 사망 시 이동 및 탐색 루프 중단
        _animator.SetTrigger("Die");
        Destroy(gameObject, 5f);
    }
}