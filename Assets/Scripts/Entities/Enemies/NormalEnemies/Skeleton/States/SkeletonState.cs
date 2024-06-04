using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonState : EnemyState
{
    protected Skeleton _skeleton;
    public SkeletonState(Skeleton skeleton, EnemyStateMachine stateMachine) : base(stateMachine)
    {
        _skeleton = skeleton;
    }
}
