using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState {

    protected PlayerController _player;
    protected PlayerStateMachine _playerStateMachine;

    public PlayerState(PlayerController player, PlayerStateMachine stateMachine)
    {
        _player = player;
        _playerStateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }

    public virtual void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType) { }

    public virtual void HandleTriggerCollision(Collider2D collision) { }

}
