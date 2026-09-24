public class RunState : IState
{
    private readonly PlayerAnimator _playerAnimator;

    private AnimationStateMachine StateMachine => _playerAnimator.AnimationStateMachine;

    public RunState(PlayerAnimator player)
    {
        _playerAnimator = player;
    }
    
    public void Enter()
    {
        _playerAnimator.PlayRun();
    }

    public void Execute()
    {
        // 움직이지 않으면 IdleState, 움직이지만 뛰지 않으면 WalkState로 전환
        if (!_playerAnimator.isMoving)
        {
            StateMachine.TransitionTo(StateMachine.IdleState);
        }
        else if (!_playerAnimator.isSprint)
        {
            StateMachine.TransitionTo(StateMachine.WalkState);
        }
    }

    public void Exit()
    {
        
    }
}
