using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    Enemy _enemy;

    public EnemyDeathState(Enemy enemy, EnemyStateMachine stateMachine) : base(stateMachine)
    {
        _enemy = enemy;
    }

    public override void EnterState()
    {
        _enemy.changeAnimation(Enemy.ENEMY_ANIMATION.Dying);
    }

    public override void ExitState()
    {

    }

    public override void FrameUpdate()
    {

    }

    public override void AnimationTriggerEvent(Enemy.ANIMATION_TRIGGER_TYPE animationTriggerType)
    {
        if (animationTriggerType == Enemy.ANIMATION_TRIGGER_TYPE.DEAD) { GameObject.Destroy(_enemy.gameObject); }
    }

    public override void HandleTriggerCollision(Collider2D collision)
    {

    }
}
