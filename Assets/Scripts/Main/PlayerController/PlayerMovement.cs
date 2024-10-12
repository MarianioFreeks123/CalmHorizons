using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float minJumpForce;
    [SerializeField] float maxJumpForce;
    [SerializeField] float terrainFriction;
    [SerializeField] float groundLinearDrag;
    [SerializeField] float airLinearDrag;
    [SerializeField] float checkGroundRadius;
    [SerializeField] float coyoteTime = 0.2f;
    [SerializeField] float inputBufferTime = 0.2f;

    [SerializeField] LayerMask[] jumpLayers;

    [Header("CHECKERS")]
    [SerializeField] bool isTouchingGround = false;
    public bool playerIsLookingLeft = false;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform checkGround;

    private Rigidbody2D rb2D; 

    private float coyoteTimeCounter;
    private float inputBufferCounter;

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
        // Create a LayerMask variable that combines the layers at index 0 and 1
        LayerMask combinedJumpLayers = jumpLayers[0] | jumpLayers[1];

        // Check if the ground is being touched using the combined layers
        isTouchingGround = Physics2D.OverlapCircle(checkGround.position, checkGroundRadius, combinedJumpLayers);
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

