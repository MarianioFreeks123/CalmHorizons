using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] float speed; // Horizontal movement speed
    [SerializeField] float maxSpeed; // Maximum horizontal speed limit

    [SerializeField] float minJumpForce; // Minimum jump force when the jump is triggered
    [SerializeField] float maxJumpForce; // Maximum jump force for dynamic jumping

    [SerializeField] float terrainFriction; // Friction applied when on ground

    [SerializeField] float groundLinearDrag; // Linear drag applied when grounded
    [SerializeField] float airLinearDrag; // Linear drag applied when in the air

    [SerializeField] float checkGroundRadius; // Radius for checking if the player is touching the ground

    [SerializeField] float coyoteTime = 0.2f; // Time allowed to jump after leaving the ground (coyote time)

    [SerializeField] float inputBufferTime = 0.2f; // Time allowed to buffer the jump input before landing

    [SerializeField] LayerMask groundLayer; // Defines what is considered as ground

    [Header("CHECKERS")]
    [SerializeField] bool isTouchingGround = false; // Boolean to check if the player is grounded
    [SerializeField] bool hasLances = true; // Example flag, not used in movement logic

    [Header("REFERENCES IN SCENE")]
    [SerializeField] SpriteRenderer spriteRenderer; // Reference to the player's sprite renderer
    [SerializeField] Transform checkGround; // Transform to check if player is grounded

    [Header("REFERENCES IN PROJECT")]
    [SerializeField] GameObject lance; // Example reference, not used in movement logic


    private Rigidbody2D rb2D; // Rigidbody2D component reference

    private bool playerIsLookingLeft = false; // Tracks the direction the player is facing

    private float coyoteTimeCounter; // Counter to track the remaining coyote time
    private float inputBufferCounter; // Counter to track the remaining input buffer time

    // Inputs
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        // Assign references
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Buffer jump input if the jump button is pressed
        if (Input.GetButtonDown("Jump"))
        {
            inputBufferCounter = inputBufferTime;
        }

        // Decrease input buffer over time
        inputBufferCounter -= Time.deltaTime;

        // Handle jump with coyote time and input buffer
        if (inputBufferCounter > 0 && (isTouchingGround || coyoteTimeCounter > 0))
        {
            Jump();
        }
    }

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        // Register Inputs
        horizontalInput = Input.GetAxis("Horizontal");

        // Apply movement
        Move();

        // Check when the player is on the ground
        CheckGround();

        // Manage coyote time counter
        if (isTouchingGround)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        //Extra fall speed
        if (rb2D.velocity.y < 0)
        {
            rb2D.velocity += Vector2.up * Physics2D.gravity.y * 1.5f * Time.deltaTime;
        }
    }

    private void Move()
    {
        // Move player horizontally
        rb2D.AddForce(Vector2.right * horizontalInput * speed);

        // Flip sprite according to movement direction
        if (horizontalInput > 0)
            playerIsLookingLeft = false;
        else if (horizontalInput < 0)
            playerIsLookingLeft = true;

        spriteRenderer.flipX = playerIsLookingLeft;

        // Limit player speed
        float speedLimit = Mathf.Clamp(rb2D.velocity.x, -maxSpeed, maxSpeed);
        rb2D.velocity = new Vector2(speedLimit, rb2D.velocity.y);

        // Adjust linear drag based on ground or air state
        ModifyLinearDrag();

        // Apply terrain friction to slow down the player if they are grounded
        if ((horizontalInput < 0.1 || horizontalInput > 0.1) && isTouchingGround)
        {
            Vector3 fixedFrictionSpeed = rb2D.velocity;
            fixedFrictionSpeed.x *= terrainFriction;
            rb2D.velocity = fixedFrictionSpeed;
        }
    }

    private void Jump()
    {
        // Apply initial jump force
        rb2D.velocity = new Vector2(rb2D.velocity.x, minJumpForce);

        // Reset coyote time counter and input buffer counter
        coyoteTimeCounter = 0;
        inputBufferCounter = 0;

        // Start coroutine for dynamic jumping control
        StartCoroutine(HandleDynamicJump());
    }

    // Coroutine to manage dynamic jump based on button hold time
    private IEnumerator HandleDynamicJump()
    {
        float jumpTime = 0f;
        float jumpDuration = 0.3f;  // Duration for dynamic jump control

        // Continue adding jump force while jump button is held
        while (Input.GetButton("Jump") && jumpTime < jumpDuration)
        {
            rb2D.velocity += new Vector2(0, (maxJumpForce - minJumpForce) * Time.deltaTime);
            jumpTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // If the button is released early and player is still going up, reduce vertical speed
        if (!Input.GetButton("Jump") && rb2D.velocity.y > 0)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y * 0.5f);
        }
    }

    // Adjust linear drag depending on whether the player is grounded or airborne
    private void ModifyLinearDrag()
    {
        if (isTouchingGround)
        {
            rb2D.drag = groundLinearDrag;
        }
        else
        {
            rb2D.drag = airLinearDrag;
        }
    }

    // Check if the player is grounded
    private void CheckGround()
    {
        // Check if the player is touching the ground by overlapping a circle at the checkGround position
        isTouchingGround = Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        if (checkGround != null)
        {
            // Draw a wire sphere to represent the ground check area
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkGround.position, checkGroundRadius);
        }
    }
}

