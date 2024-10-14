using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("INPUT")]
    [Tooltip("Makes all input snap to an integer. Prevents gamepads from walking slowly.")]
    [SerializeField] bool snapInput = true;
    [SerializeField][Range(0.01f, 0.99f)] float horizontalDeadZoneThreshold = 0.1f;
    [SerializeField][Range(0.01f, 0.99f)] float verticalDeadZoneThreshold = 0.3f;

    [Header("MOVEMENT")]
    [SerializeField] float maxSpeed = 14f;
    [SerializeField] float acceleration = 120f;
    [SerializeField] float groundDeceleration = 60f;
    [SerializeField] float airDeceleration = 30f;
    [SerializeField][Range(0f, -10f)] float groundingForce = -1.5f;

    [Header("JUMP")]
    [SerializeField] float jumpPower = 36f;
    [SerializeField] float maxFallSpeed = 40f;
    [SerializeField] float fallAcceleration = 110f;
    [SerializeField] float jumpEndEarlyGravityModifier = 3f;
    [SerializeField] float coyoteTime = 0.15f;
    [SerializeField] float jumpBuffer = 0.2f;

    [Header("CHECKERS")]
    [SerializeField] float grounderDistance = 0.05f;
    [SerializeField] Transform checkGround;
    public bool isGrounded;
    public bool playerIsLookingLeft = false;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] SpriteRenderer spriteRenderer;
    private Rigidbody2D _rb2D;

    [Header("LAYERS")]
    [Tooltip("Set this to the layer your player is on")]
    [SerializeField] LayerMask groundLayers;

    private float _time;
    private float _timeJumpWasPressed;
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _coyoteUsable;
    private bool _endedJumpEarly;    
    private Vector2 _frameVelocity;
    private float _frameLeftGrounded = float.MinValue;

    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    private void GatherInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (snapInput)
        {
            horizontalInput = Mathf.Abs(horizontalInput) < horizontalDeadZoneThreshold ? 0 : Mathf.Sign(horizontalInput);
            verticalInput = Mathf.Abs(verticalInput) < verticalDeadZoneThreshold ? 0 : Mathf.Sign(verticalInput);
        }

        if (Input.GetButtonDown("Jump"))
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();

        ApplyMovement();
    }

    private void CheckCollisions()
    {
        bool groundHit = Physics2D.OverlapCircle(checkGround.position, grounderDistance, groundLayers);
        bool ceilingHit = Physics2D.OverlapCircle(checkGround.position, grounderDistance, groundLayers);

        // Lógica para colisión en el techo
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Lógica para colisión en el suelo
        if (!isGrounded && groundHit)
        {
            isGrounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
        }
        else if (isGrounded && !groundHit)
        {
            isGrounded = false;
            _frameLeftGrounded = _time;
        }
    }

    private void HandleJump()
    {
        if (!_endedJumpEarly && !isGrounded && !Input.GetButton("Jump") && _rb2D.velocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump()) return;

        if (isGrounded || CanUseCoyote()) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = jumpPower;
    }

    private bool HasBufferedJump() => _bufferedJumpUsable && _time < _timeJumpWasPressed + jumpBuffer;
    private bool CanUseCoyote() => _coyoteUsable && !isGrounded && _time < _frameLeftGrounded + coyoteTime;

    private void HandleDirection()
    {
        if (horizontalInput == 0)
        {
            float deceleration = isGrounded ? groundDeceleration : airDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, horizontalInput * maxSpeed, acceleration * Time.fixedDeltaTime);

        // Flip sprite according to movement direction
        if (horizontalInput > 0) playerIsLookingLeft = false;

        else if (horizontalInput < 0) playerIsLookingLeft = true;

        spriteRenderer.flipX = playerIsLookingLeft;
    }

    private void HandleGravity()
    {
        if (isGrounded && _frameVelocity.y <= 0f) _frameVelocity.y = groundingForce;

        else
        {
            float inAirGravity = fallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= jumpEndEarlyGravityModifier;

            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }    

    private void ApplyMovement() => _rb2D.velocity = _frameVelocity;

    public void Bounce(float bounceForce) => _frameVelocity.y = bounceForce;
    private void OnDrawGizmos()
    {
        if (checkGround != null)
        {
            // Draw a wire sphere to represent the ground check area
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkGround.position, grounderDistance);
        }
    }
}
