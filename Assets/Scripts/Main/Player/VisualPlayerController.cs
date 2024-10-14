using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPlayerController : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] float horizontalAxisDeadZone = 0.1f;
    [SerializeField] float idleDelay = 0.05f;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] PlayerMovement playerMovement;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private float timeSinceLastMovement = 0f;
    private bool isMoving = false;

    void Start()
    {
        //Assign references
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        //ANIMATIONS
        AnimatorStatesController(horizontalInput);

        //SPRITE RENDERER
        SpriteRendererModifications();

    }

    private void AnimatorStatesController(float horizontalInput)
    {
        // Movement animation inputs
        if (Mathf.Abs(horizontalInput) > horizontalAxisDeadZone)
        {
            timeSinceLastMovement = 0f;  // Reset delay timer
            if (!isMoving)
            {
                _animator.SetBool("isMoving", true);
                isMoving = true;  // Avoid call several times
            }
        }

        else
        {
            // If there is no input start timer count
            timeSinceLastMovement += Time.deltaTime;

            //  Enter idle
            if (timeSinceLastMovement >= idleDelay && isMoving)
            {
                _animator.SetBool("isMoving", false);
                isMoving = false;
            }
        }

        // Jump animator parameters
        if (playerMovement.isGrounded) _animator.SetBool("isJumping", false);

        else _animator.SetBool("isJumping", true);
    }

    private void SpriteRendererModifications()
    {
        if (Input.GetAxis("Horizontal") < -0.1 || Input.GetAxis("Horizontal") > 0.1)
        {
            if (playerMovement.playerIsLookingLeft)
            {
                //Flip Sprite
                _spriteRenderer.flipX = true;

                //Rotate Sprite for moving aesthetic
                this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -3));
            }

            else
            {
                //Flip Sprite
                _spriteRenderer.flipX = false;

                //Rotate Sprite for moving aesthetic
                this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 3));
            }
        }

        else this.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}