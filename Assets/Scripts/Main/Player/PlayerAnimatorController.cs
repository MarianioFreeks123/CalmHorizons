using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [Header("REFERENCES IN SCENE")]
    [SerializeField] PlayerMovement PlayerMovement;

    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Moving animator parameters
        if (Input.GetAxis("Horizontal") < -0.1 || Input.GetAxis("Horizontal") > 0.1) _animator.SetBool("isMoving", true);
        else _animator.SetBool("isMoving", false);

        //Jump animator parameters
        if (PlayerMovement.isTouchingGround) _animator.SetBool("isJumping", false);
        else _animator.SetBool("isJumping", true);
    }
}
