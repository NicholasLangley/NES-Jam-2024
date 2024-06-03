using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerState
{
    public PlayerJumpingState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Jumping);
        _player.Jump();
        _playerStateMachine.changeState(_player._playerAirborneState);
    }

    public override void ExitState()
    {
        
    }

    public override void FrameUpdate()
    {
        _player._coyoteTime += Time.deltaTime;
    }

    public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {

    }
}
