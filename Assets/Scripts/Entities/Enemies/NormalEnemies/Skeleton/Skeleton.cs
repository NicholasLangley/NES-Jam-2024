using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy, IFallable
{
    public SkeletonWalkingState _skeletonWalkingState;
    public SkeletonDamageState _skeletonDamageState;

    #region IFallable
    //IFallable
    [field: SerializeField, Header("IFallable")] public BoxCollider2D _groundCollider { get; set; }

    [field: SerializeField] public bool _isGrounded { get; set; } = true;
    [field: SerializeField] public LayerMask _groundLayers { get; set; }

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        base.Awake();

        _skeletonWalkingState = new SkeletonWalkingState(this, _StateMachine);
        _skeletonDamageState = new SkeletonDamageState(this, _StateMachine);
    }

    private void Start()
    {
        _StateMachine.Initialize(_skeletonWalkingState);
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        _StateMachine.currentState.FrameUpdate();
    }


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
    }

    public void CheckIfGrounded()
    {
        _isGrounded = false;
        int rayCount = 3;
        for (int i = -1; i < rayCount - 1; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x + i * 0.9f, transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.0625f, _groundLayers);
            if (hit.collider != null)
            {
                _isGrounded = true;
                if (_currentYSpeed < 0)
                {
                     //set player height to ground height to prevent floating or clipping
                     Vector3 groundPos = transform.localPosition;
                     groundPos.y = hit.point.y;
                     transform.position = groundPos;
                     _currentYSpeed = 0; 
                }
                return;
            }
        }

        Fall();

    }

    public void Fall()
    {
        _currentYSpeed = Mathf.Max(_minYSpeed, _currentYSpeed += _yAcceleration * Time.deltaTime);
    }

    public void CheckForWallCollision()
    {
        if (_currentXSpeed == 0) { return; }

        Vector2 collisionCheckDirection;

        if (_currentXSpeed > 0) { collisionCheckDirection = Vector2.right; }
        else { collisionCheckDirection = Vector2.left; }


        int rayCount = 5;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y + i);
            //prevent minor floor clipping from stopping movement
            if (i == 0)
            {
                rayOrigin.y += 0.05f;
            }

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, collisionCheckDirection, 1.01f, _groundLayers);
            if (hit.collider != null) { _currentXSpeed = 0; return; }
        }

    }

    public void CheckForClff()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x + Vector2.left.x * transform.localScale.x * 0.9f, transform.position.y);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 0.0625f, _groundLayers);
        if (hit.collider == null)
        {
            FlipMovement();
        }
    }

    public void CheckForWallFlip()
    {

        Vector2 collisionCheckDirection;

        collisionCheckDirection = Vector2.left;
        collisionCheckDirection.x *= transform.localScale.x;


        int rayCount = 2;
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y + i + 0.5f);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, collisionCheckDirection, 1.01f, _groundLayers);
            if (hit.collider != null) 
            {
                FlipMovement();
                return; 
            }
        }

    }

    public void FlipMovement()
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
    #endregion
}
