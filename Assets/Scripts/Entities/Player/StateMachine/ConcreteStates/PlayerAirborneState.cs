using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    public PlayerAirborneState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        
    }

    public override void ExitState()
    {
        _player.UnpauseAnimation();
    }

    public override void FrameUpdate()
    {
        if (_player._currentYSpeed <= 0.15f)
        {
            _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Falling);
            _player.PauseAnimation();
            _player._isJumping = false;
        }

        if (_player._currentYSpeed == 0 && _player._isGrounded)
        {
            _playerStateMachine.changeState(Input.GetAxisRaw("Vertical") < 0 ? _player._playerCrouchingState : _player._currentXSpeed != 0 ? _player._playerWalkingState : _player._playerIdleState);
        }
        else { AirControl(); }
    }

    public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {

    }

    private void AirControl()
    {
        float accelertionAmount = _player._xAcceleration * _player._airAccelrationFactor * Time.deltaTime;
        float horizontalAccel = Input.GetAxisRaw("Horizontal") * accelertionAmount;
        //accelerate
        if (horizontalAccel != 0) { _player._currentXSpeed = Mathf.Clamp(_player._currentXSpeed += horizontalAccel, -_player._maxXSpeed, _player._maxXSpeed); }

        _player.checkDirectionToFace(horizontalAccel);
        _player.Move();
    }
}
