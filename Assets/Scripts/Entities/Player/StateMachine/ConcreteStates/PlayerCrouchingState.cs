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
        _player.checkDirectionToFace(Input.GetAxisRaw("Horizontal"));

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
        else if (Input.GetButtonDown("Fire1")) 
        {
            _playerStateMachine.changeState(_player._playerStabCrouchState);
        }
        else { _player.Decelerate(); _player.Move(); }
    }

    public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {

    }

    public override void HandleTriggerCollision(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("EnemyAttack"))
        {
            _player._damageBounceDirectionIsRight = collision.transform.localPosition.x < _player.transform.position.x;

            _playerStateMachine.changeState(_player._playerDamagedState);
        }
    }
}
