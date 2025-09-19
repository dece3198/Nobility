using UnityEngine;

public enum State
{
    Idle, Walk, Hit, Attack, Die
}

public class PlayerState : MonoBehaviour
{
    public float damage;
    public float cDamage;
    public float cPercent;

    public State playerState;
    public StateMachine<State, PlayerState> stateMachine = new StateMachine<State, PlayerState>();


    public void ChangeState(State state)
    {
        stateMachine.ChangeState(state);
        playerState = state;
    }
}
