using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStabState : PlayerState
{
    public PlayerStabState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void EnterState()
    {
        _player.changeAnimation(PlayerController.PLAYER_ANIMATION.Attacking);
        
    }

    public override void ExitState()
    {
        _player.disableAttackHitBox();
    }

    public override void FrameUpdate()
    {
        _player.checkDirectionToFace(Input.GetAxisRaw("Horizontal"));
        if (_player._isGrounded && !_player._isJumping){ _player._currentXSpeed = 0; }
        _player.Move();
    }

    public override void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {
        if (animationTriggerType == PlayerController.ANIMATION_TRIGGER_TYPE.ATTACKING_FRAME) { _player.enableAttackHitBox(); }
        else if (animationTriggerType == PlayerController.ANIMATION_TRIGGER_TYPE.FINISHED_ATTACKING) 
        {
            _playerStateMachine.changeState(_player._playerAirborneState);
        }
    }
}
