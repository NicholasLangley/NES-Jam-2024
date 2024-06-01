using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable, IHealable, IMoveable, IJumpable, IFallable
{
    #region speed variables

    [Header ("x speed")]
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

    #region animations

    Animator _animator;
    public enum PLAYER_ANIMATION {Idle, Walking, Crouching, Jumping, Falling}
    PLAYER_ANIMATION _currentAnimation = PLAYER_ANIMATION.Idle;
    SpriteRenderer _renderer;

    #endregion

    #region interface fields
    //IDamageable  IHealable
    
    public int _health { get; set; }

    
    [field: SerializeField, Header("IDamageable")] public int _maxHealth { get; set; }

    //IMoveable
    public bool _isFacingRight { get; set; } = true;


    //IFallable
    [field: SerializeField, Header("IFallable")] public BoxCollider2D _groundCollider { get; set; }

    [field: SerializeField] public bool _isGrounded { get; set; } = true;
    [field: SerializeField] public LayerMask _groundLayers { get; set; }

    //IJumpable 
    public bool _isJumping { get; set; }

    #endregion

    #region State Machine States
    public PlayerStateMachine _StateMachine { get; set; }
    public PlayerIdleState _playerIdleState { get; set; }
    public PlayerWalkingState _playerWalkingState { get; set; }
    public PlayerCrouchingState _playerCrouchingState { get; set; }

    public PlayerJumpingState _playerJumpingState { get; set; }

    public PlayerAirborneState _playerAirborneState { get; set; }

    #endregion

    void Awake()
    {
        _StateMachine = new PlayerStateMachine();
        _playerIdleState = new PlayerIdleState(this, _StateMachine);
        _playerWalkingState = new PlayerWalkingState(this, _StateMachine);
        _playerCrouchingState = new PlayerCrouchingState(this, _StateMachine);
        _playerJumpingState = new PlayerJumpingState(this, _StateMachine);
        _playerAirborneState = new PlayerAirborneState(this, _StateMachine);
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _currentAnimation = PLAYER_ANIMATION.Idle;

        _currentXSpeed = 0;
        _currentYSpeed = 0;
        _health = _maxHealth;

        _StateMachine.Initialize(_playerIdleState);
    }

    // Update is called once per frame
    void Update()
    {
        _StateMachine.currentState.FrameUpdate();
    }

    #region animations
    public void changeAnimation(PLAYER_ANIMATION nextAnimation)
    {
        if (_currentAnimation == nextAnimation) { return; }

        _animator.Play(System.Enum.GetName(typeof(PLAYER_ANIMATION), nextAnimation));
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

    #region Damageable

    public void Damage(int damage)
    {
        _health -= damage;
        if (_health < 0) { Kill(); }
    }

    public void Kill()
    {
        GameObject.Destroy(gameObject);
    }

    #endregion

    #region Healable

    public void Heal(int heal)
    {
        _health += heal;
        _health = Mathf.Min(_health, _maxHealth);
    }

    #endregion

    #region Movement
    //Interface implementations
    public void Move()
    {
        CheckIfGrounded();

        Vector3 nextPos = transform.position;
        nextPos.x += _currentXSpeed * Time.deltaTime;
        nextPos.y += _currentYSpeed * Time.deltaTime;
        transform.position = nextPos;
    }

    public void checkDirectionToFace(float accel)
    {
        if (accel != 0) 
        {
            _isFacingRight = accel > 0;
            _renderer.flipX = !_isFacingRight; 
        }
    }

    public void CheckIfGrounded()
    {
         _isGrounded = _groundCollider.IsTouchingLayers(_groundLayers) && !_isJumping;


        if (_isGrounded) { if (_currentYSpeed < 0) { _currentYSpeed = 0; } }
        else { Fall(); }
    }

    //extra
    public void Decelerate()
    {
        float accelertionAmount = _xAcceleration * Time.deltaTime;
        if (_currentXSpeed > 0) { _currentXSpeed = Mathf.Max(0, _currentXSpeed -= accelertionAmount / 2.0f); }
        else if (_currentXSpeed < 0) { _currentXSpeed = Mathf.Min(_currentXSpeed += accelertionAmount / 2.0f, 0); }
    }

    public void Fall()
    {
        _currentYSpeed = Mathf.Max(_minYSpeed, _currentYSpeed += _yAcceleration * Time.deltaTime);
    }

    public void Jump()
    {
        _currentYSpeed = _intialJumpVelocity;
        _isJumping = true;
    }

    #endregion

    #region animationTriggers

    public enum ANIMATION_TRIGGER_TYPE { Damaged }

    void AnimationTriggerEvent(ANIMATION_TRIGGER_TYPE animationTriggerType)
    {
        _StateMachine.currentState.AnimationTriggerEvent(animationTriggerType);
    }

    #endregion

}
