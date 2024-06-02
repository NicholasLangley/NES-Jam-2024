using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchingState : PlayerState
{
    public PlayerCrouchingState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Crouching);
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        if (Input.GetAxisRaw("Vertical") >= 0)
        {
            _playerStateMachine.changeState(_player._currentXSpeed == 0 ? _player._playerIdleState : _player._playerWalkingState);
        }
        else if (!_player._isGrounded)
        {
            _playerStateMachine.changeState(_player._playerAirborneState);
        } 
        else if (Input.GetButtonDown("Jump"))
        {
            _playerStateMachine.changeState(_player._playerJumpingState);
        }
        else { _player.Decelerate(); _player.Move(); }
    }

    public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {

    }
}
