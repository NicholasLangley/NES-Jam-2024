using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState { get; set; }

    public void Initialize(PlayerState startingState)
    {
        currentState = startingState;
        currentState.EnterState();
    }

    public void changeState(PlayerState nextState)
    {
        currentState.ExitState();
        currentState = nextState;
        currentState.EnterState();
    }
}
