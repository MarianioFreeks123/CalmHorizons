using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] float horizontalAxisDeadZone = 0.1f;
    [SerializeField] float idleDelay = 0.05f;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] PlayerMovement PlayerMovement;

    private Animator _animator;
    private float timeSinceLastMovement = 0f;
    private bool isMoving = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

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
        if (PlayerMovement.isGrounded) _animator.SetBool("isJumping", false);

        else _animator.SetBool("isJumping", true);
    }
}