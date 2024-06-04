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
        else if (Input.GetButtonDown("Jump"))
        {
            _playerStateMachine.changeState(_player._playerJumpingState);
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            _playerStateMachine.changeState(_player._playerStabState);
        }
        else { Walk(); }
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
