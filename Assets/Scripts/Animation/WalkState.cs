public class WalkState : IState
{
    private readonly PlayerAnimator _playerAnimator;
    private readonly WalkingIK _walkingIK;

    private AnimationStateMachine StateMachine => _playerAnimator.AnimationStateMachine;

    public WalkState(PlayerAnimator playerAnimator, WalkingIK walkingIK)
    {
        _playerAnimator = playerAnimator;
        _walkingIK = walkingIK;
    }
    
    public void Enter()
    {
        _playerAnimator.PlayWalk();
        _walkingIK.ActivateIK();
    }

    public void Execute()
    {
        if (!_playerAnimator.isMoving)
        {
            StateMachine.TransitionTo(StateMachine.IdleState);
        }
        else if (_playerAnimator.isSprint)
        {
            StateMachine.TransitionTo(StateMachine.RunState);
        }
    }

    public void Exit()
    {
        _walkingIK.DeactivateIK();
    }
}
