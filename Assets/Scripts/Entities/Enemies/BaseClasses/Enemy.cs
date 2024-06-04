using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IAnimateable
{

    #region State Machine 
    public EnemyStateMachine _StateMachine { get; set; }

    //Death
    public EnemyState _deathState { get; set; }

    #endregion

    #region IDamageable
    public int _health { get; set; }
    [field: SerializeField, Header("IDamageable")] public int _maxHealth { get; set; }

    [field: SerializeField] public bool _damageBounceDirectionIsRight { get; set; }
    [field: SerializeField] public float _bounceYVelocity { get; set; }
    [field: SerializeField] public float _bounceSpeed { get; set; }
    #endregion

    #region speed variables

    [Header("x speed")]
    public float _maxXSpeed = 16.0f; //16 means it takes about 4 seconds to cross a screenwidth
    public float _xAcceleration = 128.0f;
    public float _currentXSpeed;
    public float _airAccelrationFactor = 0.25f;

    [Header("y speed")]
    public float _minYSpeed = -16.0f;
    public float _yAcceleration = -8.0f;
    public float _currentYSpeed;

    public float _intialJumpVelocity = 16.0f;

    #endregion

    public Animator _animator;

    protected void Awake()
    {
        
        _StateMachine = new EnemyStateMachine();
        _deathState = new EnemyDeathState(this, _StateMachine);
        _animator = GetComponent<Animator>();

        //child class should create individual states
    }
    // Start is called before the first frame update
    protected void Start()
    {
        //child class should initialize state machine
        _health = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region IMoveable
    public void Damage(int damage)
    {
        _health -= damage;
        if(_health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        _StateMachine.changeState(_deathState);
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _StateMachine.currentState.HandleTriggerCollision(collision);
    }

    #region animations

    public enum ANIMATION_TRIGGER_TYPE { ATTACKING_FRAME, FINISHED_ATTACKING, DEAD}

    void AnimationTriggerEvent(ANIMATION_TRIGGER_TYPE animationTriggerType)
    {
        _StateMachine.currentState.AnimationTriggerEvent(animationTriggerType);
    }

    public enum ENEMY_ANIMATION { Standing, Walking, Jumping, Attacking, Damaged, Dying}

    ENEMY_ANIMATION _currentAnimation;
    public void changeAnimation(ENEMY_ANIMATION nextAnimation)
    {
        if (_currentAnimation == nextAnimation) { return; }

        _animator.Play(System.Enum.GetName(typeof(ENEMY_ANIMATION), nextAnimation));
        _currentAnimation = nextAnimation;
    }

    public void PauseAnimation()
    {
        _animator.speed = 0;
    }

    public void UnpauseAnimation()
    {
        _animator.speed = 1.0f;
    }

    #endregion
}
