using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonWalkingState : SkeletonState
{
    public SkeletonWalkingState(Skeleton skeleton, EnemyStateMachine stateMachine) : base(skeleton, stateMachine)
    {
    }

    public override void EnterState()
    {
        _skeleton.changeAnimation(Enemy.ENEMY_ANIMATION.Walking);
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {
        _skeleton.CheckForClff();
        _skeleton.CheckForWallFlip();
        Walk();
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

    private void Walk()
    {
        float accelertionAmount = _skeleton._xAcceleration * Time.deltaTime;
        float horizontalAccel = Vector2.left.x * _skeleton.transform.localScale.x * accelertionAmount;
        //accelerate
        _skeleton._currentXSpeed = Mathf.Clamp(_skeleton._currentXSpeed += horizontalAccel, -_skeleton._maxXSpeed, _skeleton._maxXSpeed);

        _skeleton.Move();
    }

}