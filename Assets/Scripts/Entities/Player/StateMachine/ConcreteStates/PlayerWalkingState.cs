using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    public PlayerWalkingState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Walking);
        FrameUpdate();
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        if (Input.GetAxisRaw("Vertical") < 0) { _playerStateMachine.changeState(_player._playerCrouchingState); }
        else if (!_player._isGrounded)
        {
            _playerStateMachine.changeState(_player._playerAirborneState);
        }
        else if (Input.GetAxisRaw("Jump") > 0)
        {
            _playerStateMachine.changeState(_player._playerJumpingState);
        }
        else { Walk(); }
    }

    public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {

    }

    private void Walk()
    {
        float accelertionAmount = _player._xAcceleration * Time.deltaTime;
        float horizontalAccel = Input.GetAxisRaw("Horizontal") * accelertionAmount;
        //accelerate
        if (horizontalAccel != 0) { _player._currentXSpeed = Mathf.Clamp(_player._currentXSpeed += horizontalAccel, -_player._maxXSpeed, _player._maxXSpeed); }
        //decelerate
        else
        {
            _player.Decelerate();
        }

        //handle animation changes
        if (_player._currentXSpeed == 0 && horizontalAccel == 0) { _playerStateMachine.changeState(_player._playerIdleState); }
        else
        {
            _player.checkDirectionToFace(horizontalAccel);
            _player.Move();
        }
    }
}