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

    Camera camera;

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

        camera = Camera.main;
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
        CheckForWallCollision();

        Vector3 nextPos = transform.position;
        nextPos.x += _currentXSpeed * Time.deltaTime;
        nextPos.y += _currentYSpeed * Time.deltaTime;
        transform.position = nextPos;

        UpdateCameraPosition();
    }

    public void UpdateCameraPosition()
    {
        Vector3 newPos = camera.transform.position;
        newPos.x = transform.position.x;
        camera.transform.position = newPos;
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
       /* _isGrounded = _groundCollider.IsTouchingLayers(_groundLayers) && !_isJumping;


       if (_isGrounded) 
        { 
            if (_currentYSpeed < 0) { _currentYSpeed = 0; }
            //push player up to not be clipping in floor
            Vector3 fixClippingPos = transform.localPosition;
            fixClippingPos.y = Mathf.Ceil(fixClippingPos.y);
            transform.position = fixClippingPos;
        }
       else { Fall(); }*/

        _isGrounded = false;
        int rayCount = 3;
        for (int i = -1; i < rayCount - 1; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x + i * 0.9f, transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.0625f, _groundLayers);
            if (hit.collider != null) {
                _isGrounded = true;
                //set player height to ground height to prevent floating or clipping
                Vector3 groundPos = transform.localPosition;
                groundPos.y = hit.point.y;
                transform.position = groundPos;
                if (_currentYSpeed < 0) { _currentYSpeed = 0; }
                return; 
            }
        }

        Fall();

    }

    public void CheckForWallCollision()
    {
        if (_currentXSpeed == 0) { return; }

        Vector2 collisionCheckDirection;

        if (_currentXSpeed > 0) { collisionCheckDirection = Vector2.right; }
        else { collisionCheckDirection = Vector2.left; }


        int rayCount = 5;
        for(int i = 0; i <= rayCount; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y + i);
            //prevent minor floor clipping from stopping movement
            if (i == 0) {
                rayOrigin.y += 0.25f; 
            }

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, collisionCheckDirection, 1.01f, _groundLayers);
            if (hit.collider != null) { _currentXSpeed = 0; return; }
        }

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
        _currentYSpeed = _intialJumpVelocity + Mathf.Abs(_currentXSpeed)/_maxXSpeed * _intialJumpVelocity / 6.0f;
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
