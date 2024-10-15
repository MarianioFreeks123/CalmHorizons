using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public delegate void PlayerMovementDelegate();
    public event PlayerMovementDelegate Bend;

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
    [SerializeField] private float lastStepTime;
    [SerializeField] private float StepInterval = 0.5f;

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
    public Vector2 playerPosition;


   
    
    private AudioSource _audioSource;
    [Header("AUDIO FX")]
    [SerializeField] AudioClip _jumpSound;
    [SerializeField] AudioClip _walkSoundTerrain;
    [SerializeField] AudioClip _walkSoundWood;
    [SerializeField] AudioClip _walkSoundStone;
    [SerializeField] AudioClip _walkSoundSnow;

    [Header("AUDIO VOLUME")]
    [Range(0.0f, 1.0f)]
    [SerializeField] float jumpSoundVolume;
    [Range(0.0f, 1.0f)]
    [SerializeField] float walkSoundWoodVolume;
    [Range(0.0f, 1.0f)]
    [SerializeField] float walkSoundStoneVolume;
    [Range(0.0f, 1.0f)]
    [SerializeField] float walkSoundSnowVolume;

    [Header("REFERENCES IN SCENE")]
    private Rigidbody2D _rb2D;

    [Header("LAYERS")]
    [Tooltip("Set this to the layer your player is on")]
    [SerializeField] LayerMask groundLayers;    
    
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _coyoteUsable;
    private bool _endedJumpEarly;

    private Vector2 _frameVelocity;

    private float _frameLeftGrounded = float.MinValue;
    private float _time;
    private float horizontalInput;
    private float verticalInput;
    private float _timeJumpWasPressed;    

    void Start()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        lastStepTime = Time.time;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
        
        //STEPS SOUNDS SCRIPT
        if (isGrounded && _frameVelocity.magnitude > horizontalDeadZoneThreshold)
        {
            RaycastHit walkingHit;
            //Verifica si tiene la velocidad suficiente desde el último paso
            if (Time.time - lastStepTime > StepInterval )
            {
                if (Physics.Raycast(transform.position,Vector3.down,out walkingHit, 10f))
                {
                    if (walkingHit.collider.CompareTag("Terrain"))
                    {
                        _audioSource.PlayOneShot(_walkSoundTerrain);
                        lastStepTime = Time.time;
                    }

                    if (walkingHit.collider.CompareTag("Snow"))
                    {
                        
                        lastStepTime = Time.time;
                    }
                }
            }
            
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        HandleJump();
        HandleDirection();
        HandleBend();
        HandleGravity();
        ApplyMovement();
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

    private void HandleBend()
    {
        if (Input.GetKey(KeyCode.S)) Bend?.Invoke();    
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
    }

    private void HandleGravity()
    {
        if (isGrounded && _frameVelocity.y <= 0f) _frameVelocity.y = groundingForce; // Aplica una pequeña fuerza hacia abajo si está en el suelo

        else
        {
            float inAirGravity = fallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= jumpEndEarlyGravityModifier;

            // Aplica la gravedad en el aire
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    private void ApplyMovement() => _rb2D.velocity = _frameVelocity;
    
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
