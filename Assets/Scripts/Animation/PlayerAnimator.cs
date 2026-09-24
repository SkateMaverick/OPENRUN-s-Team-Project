using Player.InputActions;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool isSprint;
    
    [SerializeField] private float blendDuration = 0.15f; // 애니메이션 전환 간 블렌딩 시간
    [SerializeField] private float moveThreshold = 0.1f; // Idle 상태에서 벗어날 최소 움직임

    private Animator _animator;
    private AnimationStateMachine _animationStateMachine;
    private WalkingIK _walkingIK;

    private Rigidbody _rigidbody;
    private int _idleHash = Animator.StringToHash("Base Layer.Idle");
    private int _walkHash = Animator.StringToHash("Base Layer.Walk");
    private int _runHash = Animator.StringToHash("Base Layer.Run");

    public void PlayIdle() => _animator.CrossFadeInFixedTime(_idleHash, blendDuration);
    public void PlayWalk() => _animator.CrossFadeInFixedTime(_walkHash, blendDuration);
    public void PlayRun() => _animator.CrossFadeInFixedTime(_runHash, blendDuration);

    public AnimationStateMachine AnimationStateMachine => _animationStateMachine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _walkingIK = GetComponent<WalkingIK>();
        
        _animationStateMachine = new AnimationStateMachine(this, _walkingIK);
    }

    private void Start()
    {
        _animationStateMachine.Initialize(_animationStateMachine.IdleState);
    }

    private void Update()
    {
        Vector3 velocity = _rigidbody.linearVelocity;
        float speed = new Vector2(velocity.x, velocity.z).magnitude;

        isMoving = speed >= moveThreshold;
        isSprint = isMoving && PlayerInputReader.Instance.SprintInput;
        
        _animationStateMachine.Execute();
    }
}
