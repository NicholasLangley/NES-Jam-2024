using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDamageState : SkeletonState
{
    public SkeletonDamageState(Skeleton skeleton, EnemyStateMachine stateMachine) : base(skeleton, stateMachine)
    {
    }

    public override void EnterState()
    {
        _skeleton.Damage(1);
        if (_skeleton._health <= 0) { return; }
        _skeleton.changeAnimation(Enemy.ENEMY_ANIMATION.Damaged);
        Bounce();
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        if (_skeleton._currentYSpeed == 0 && _skeleton._isGrounded) { _enemyStateMachine.changeState(_skeleton._skeletonWalkingState); }
        else { _skeleton.Move(); }
    }

    public override void AnimationTriggerEvent(Enemy.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {
        
    }

    public override void HandleTriggerCollision(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            _skeleton._damageBounceDirectionIsRight = collision.transform.localPosition.x > 0;
            Vector3 oldOrientation = _skeleton.transform.localScale;
            oldOrientation.x = collision.transform.localPosition.x > 0 ? 1 : -1;
            _skeleton.transform.localScale = oldOrientation;

            _enemyStateMachine.changeState(_skeleton._skeletonDamageState);
        }
    }


    private void Bounce()
    {
        _skeleton._currentYSpeed = _skeleton._bounceYVelocity;
        _skeleton._currentXSpeed = _skeleton._damageBounceDirectionIsRight  ? _skeleton._bounceSpeed : -_skeleton._bounceSpeed;
    }


}
