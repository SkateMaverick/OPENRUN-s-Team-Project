using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPun, IDamageable
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public bool isDead;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    // 변경점: 외부(RPC)에서 확실히 찾을 수 있도록 public으로 변경
    [PunRPC]
    public void ApplyUpdateHealth(float newHealth) 
    {
        CurrentHealth = newHealth;
        
        // 일반 클라이언트들은 여기서 갱신된 체력을 바탕으로 사망 체크를 합니다.
        if (CurrentHealth <= 0f && !isDead)
        {
            Die();
        }
    }
    
    // 이 함수는 '마스터 클라이언트'의 권한으로 총을 맞았을 때 호출됩니다. (Raycast 판정 등)
    [PunRPC]
    public void TakeDamage(float damage)
    {
        // 1. 데미지 계산은 오직 마스터 클라이언트만 독점합니다.
        if (PhotonNetwork.IsMasterClient)
        {
            if (isDead) return; // 이미 죽었다면 무시

            CurrentHealth -= damage; 
        
            // 2. 버그 수정: damage가 아니라 계산이 끝난 'CurrentHealth'를 넘겨줍니다.
            photonView.RPC("ApplyUpdateHealth", RpcTarget.Others, CurrentHealth); 
            
            // 주의: TakeDamage를 RPC로 또 쏘지 않습니다! (중복 실행 방지)
            
            // 3. 마스터 클라이언트 자신의 사망 체크
            if (CurrentHealth <= 0f) 
            {
                CurrentHealth = 0f;
                Die();
            }
        }
    }
    
    protected virtual void Die()
    {
        isDead = true;
        // 사망 애니메이션, 이펙트, 오브젝트 파괴 로직 등이 들어갈 자리
    }
}