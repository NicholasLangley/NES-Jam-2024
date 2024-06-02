using System.Collections;
using System.Collections.Generic;
using UnityEngine;




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{

    protected Enemy _enemy;
    protected EnemyStateMachine _enemyStateMachine;

    public EnemyState(Enemy enemy, EnemyStateMachine stateMachine)
    {
        _enemy = enemy;
        _enemyStateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }

    public virtual void AnimationTriggerEvent(PlayerController.ANIMATION_TRIGGER_TYPE animationTriggerType) { }

}
