public class IdleState : IState
{
    private readonly PlayerAnimator _playerAnimator;
    
    private AnimationStateMachine StateMachine => _playerAnimator.AnimationStateMachine;

    public IdleState(PlayerAnimator player)
    {
        _playerAnimator = player;
    }
    
    public void Enter()
    {
        _playerAnimator.PlayIdle();
    }

    public void Execute()
    {
        // 달리면 RunState, 걸으면 WalkState로 전환
        if (_playerAnimator.isMoving)
        {
            StateMachine.TransitionTo(_playerAnimator.isSprint ? StateMachine.RunState : StateMachine.WalkState);
        }
    }

    public void Exit()
    {
        
    }
}
