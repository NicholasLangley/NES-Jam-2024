using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Idle);
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {

        if (Input.GetAxisRaw("Vertical") < 0) { _playerStateMachine.changeState(_player._playerCrouchingState); }
        else if (Input.GetAxisRaw("Horizontal") != 0) { _playerStateMachine.changeState(_player._playerWalkingState); }
        else if (!_player._isGrounded)
        {
            _playerStateMachine.changeState(_player._playerAirborneState);
        }
        else if (Input.GetAxisRaw("Jump") > 0)
        {
            _playerStateMachine.changeState(_player._playerJumpingState);
        }
    }

    public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {

    }
}