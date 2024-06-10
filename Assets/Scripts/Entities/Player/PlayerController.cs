using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable, IHealable, IMoveable, IJumpable, IFallable, IAnimateable
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

    [Header ("Animator")]
    [SerializeField]
    Animator _animator;
    public enum PLAYER_ANIMATION {Idle, Walking, Crouching, Jumping, Falling, Attacking, CrouchAttacking, Damaged }
    PLAYER_ANIMATION _currentAnimation = PLAYER_ANIMATION.Idle;
    [SerializeField]
    SpriteRenderer _renderer;

    #endregion

    #region interface fields
    //IDamageable  IHealable
    

    public int _health { get; set; }
    [Header("IDamageable /  IHealable")]
    [SerializeField]
    HealthBar _healthBar;

    [field: SerializeField] public bool _damageBounceDirectionIsRight { get; set; }
    [field: SerializeField] public float _bounceYVelocity { get; set; }
    [field: SerializeField] public float _bounceSpeed { get; set; }


    [field: SerializeField, Header("IDamageable")] public int _maxHealth { get; set; }

    //IMoveable
    public bool _isFacingRight { get; set; } = true;


    //IFallable
    [field: SerializeField, Header("IFallable")] public BoxCollider2D _groundCollider { get; set; }

    [field: SerializeField] public bool _isGrounded { get; set; } = true;
    [field: SerializeField] public LayerMask _groundLayers { get; set; }

    //IJumpable 
    public bool _isJumping { get; set; }

    [field: SerializeField, Header("IJumpable")] public float _coyoteTime { get; set; } = 0.1f;

    public float _coyoteTimer { get; set; }

    #endregion

    #region State Machine States
    public PlayerStateMachine _StateMachine { get; set; }
    public PlayerIdleState _playerIdleState { get; set; }
    public PlayerWalkingState _playerWalkingState { get; set; }
    public PlayerCrouchingState _playerCrouchingState { get; set; }

    public PlayerJumpingState _playerJumpingState { get; set; }

    public PlayerAirborneState _playerAirborneState { get; set; }

    public PlayerStabState _playerStabState { get; set; }
    public PlayerStabCrouchingState _playerStabCrouchState { get; set; }

    public PlayerDamagedState _playerDamagedState { get; set; }

    #endregion

    #region Hitboxes

    [SerializeField]
    BoxCollider2D _attackHitBox, _crouchAttackHitBox;

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
        _playerStabState = new PlayerStabState(this, _StateMachine);
        _playerStabCrouchState = new PlayerStabCrouchingState(this, _StateMachine);
        _playerDamagedState = new PlayerDamagedState(this, _StateMachine);
    }

    // Start is called before the first frame update
    void Start()
    {

        _currentAnimation = PLAYER_ANIMATION.Jumping;
        changeAnimation(PLAYER_ANIMATION.Idle);

        _currentXSpeed = 0;
        _currentYSpeed = 0;
        _health = _maxHealth;

        _StateMachine.Initialize(_playerIdleState);

        camera = Camera.main;

        for(int i = 0; i < _maxHealth; i += 2)
        {
            _healthBar.addHeart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        _StateMachine.currentState.FrameUpdate();
    }



    #region Damageable

    public void Damage(int damage)
    {
        _health -= damage;
        _healthBar.updateHealthBar(_health);
        if (_health <= 0) { Kill(); }
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

            //flip hitboxes
            Vector3 nextPos = _attackHitBox.transform.localPosition;
            nextPos.x =  _isFacingRight ? Mathf.Abs(nextPos.x) : -1.0f * Mathf.Abs(nextPos.x);
            _attackHitBox.transform.localPosition = nextPos;

            nextPos = _crouchAttackHitBox.transform.localPosition;
            nextPos.x = _isFacingRight ? Mathf.Abs(nextPos.x) : -1.0f * Mathf.Abs(nextPos.x);
            _crouchAttackHitBox.transform.localPosition = nextPos;
        }
    }

    public void CheckIfGrounded()
    {
        _isGrounded = false;
        int rayCount = 3;
        for (int i = -1; i < rayCount - 1; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x + i * 0.9f, transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.0625f, _groundLayers);
            if (hit.collider != null) {
                _isGrounded = true;
                if (_currentYSpeed < 0)
                {
                    //set player height to ground height to prevent floating or clipping
                    Vector3 groundPos = transform.localPosition;
                    groundPos.y = hit.point.y;
                    transform.position = groundPos;
                    _currentYSpeed = 0; 
                }
                _coyoteTimer = 0.0f;
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
                rayOrigin.y += 0.05f; 
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

    public enum ANIMATION_TRIGGER_TYPE { ATTACKING_FRAME, FINISHED_ATTACKING }

    void AnimationTriggerEvent(ANIMATION_TRIGGER_TYPE animationTriggerType)
    {
        _StateMachine.currentState.AnimationTriggerEvent(animationTriggerType);
    }

    #endregion

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

    #region hitbox management

    public void enableAttackHitBox()
    {
        _attackHitBox.enabled = true;
    }

    public void disableAttackHitBox()
    {
        _attackHitBox.enabled = false;
    }

    public void enableCrouchAttackHitBox()
    {
        _crouchAttackHitBox.enabled = true;
    }

    public void disableCrouchAttackHitBox()
    {
        _crouchAttackHitBox.enabled = false;
    }

    #endregion


    private void OnTriggerEnter2D(Collider2D collision)
    {
        _StateMachine.currentState.HandleTriggerCollision(collision);
    }

}
