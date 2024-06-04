using System.Collections;
using System.Collections.Generic;
using UnityEngine;




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{

    protected EnemyStateMachine _enemyStateMachine;

    public EnemyState(EnemyStateMachine stateMachine)
    {
        _enemyStateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }

    public virtual void AnimationTriggerEvent(Enemy.ANIMATION_TRIGGER_TYPE animationTriggerType) { }

    public virtual void HandleTriggerCollision(Collider2D collision) { }

}
