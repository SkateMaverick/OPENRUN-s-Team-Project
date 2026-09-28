public class AnimationStateMachine
{
    public IState CurrentState { get; private set; }

    public IdleState IdleState;
    public WalkState WalkState;
    public RunState RunState;

    public AnimationStateMachine(PlayerAnimator playerAnimator, WalkingIK walkingIK)
    {
        IdleState = new IdleState(playerAnimator);
        WalkState = new WalkState(playerAnimator, walkingIK);
        RunState = new RunState(playerAnimator);
    }
    
    public void Initialize(IState startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }

    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
    }

    public void Execute()
    {
        if (CurrentState != null)
        {
            CurrentState.Execute();
        }
    }
}
