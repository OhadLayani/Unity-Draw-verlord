using UnityEngine;

public abstract class State
{
    [SerializeField] protected States states;
    protected readonly StateMachine machine;

    public virtual void EnterState() { }
    public virtual void UpdateState() { }
    public virtual void ExitState() { }
}
