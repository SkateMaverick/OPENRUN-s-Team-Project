using System;
using UnityEngine;

public class SlimeAnimation : MonoBehaviour
{
    /// <summary> 메모장
    /// 1. 가만히 있을 때 2. 움직일 때 3. 맞을 때 4. 죽을 때
    /// </summary>
    
    private Animator _enemyAnimator;
    private LivingEntity _livingEntity;

    private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int _hitHash = Animator.StringToHash("Hit");
    private readonly int _isDeadHash = Animator.StringToHash("IsDead");

    private void Awake()
    {
        #region 초기화
        _enemyAnimator = GetComponent<Animator>();
        _livingEntity = GetComponent<LivingEntity>();

        //_livingEntity.OnHit += PlayHit;
        //_livingEntity.OnDeath += PlayDie;

        #endregion
    }

    private void Update()
    {
        
    }

    //private void SetMoveSpeed(float speed) => _enemyAnimator.SetFloat(_moveSpeedHash, speed);
    private void PlayHit() => _enemyAnimator.SetTrigger(_hitHash);
    private void PlayDie() => _enemyAnimator.SetBool(_isDeadHash, true);
}
