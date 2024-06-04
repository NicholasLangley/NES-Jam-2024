using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamagedState : PlayerState
{
    public PlayerDamagedState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void EnterState()
    {
        _player.Damage(1);
        if (_player._health <= 0){ return; }
        _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Damaged);
        Bounce();
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        if (_player._currentYSpeed == 0 && _player._isGrounded) { _playerStateMachine.changeState(_player._playerAirborneState); }
        else { _player.Move(); }
    }


    public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {

    }

    public override void HandleTriggerCollision(Collider2D collision)
    {

    }

    private void Bounce()
    {
        _player._currentYSpeed = _player._bounceYVelocity;
        _player._currentXSpeed = _player._damageBounceDirectionIsRight ? _player._bounceSpeed : -_player._bounceSpeed;
        _player.checkDirectionToFace(_player._currentXSpeed);
    }
}
