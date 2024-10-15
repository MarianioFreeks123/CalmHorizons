using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteTrap : MonoBehaviour
{    
    private enum StalactiteTrapState {isFixed, isFalling, isAnchored};

    [Header("PARAMETERS")]
    [SerializeField] float fallSpeed;

    [Header("CHECKERS")]
    [SerializeField] private StalactiteTrapState stalactiteTrapState;
    [SerializeField] float minYPlayerPositionToFall;    

    private Vector2 playerPosition;
    private Rigidbody2D rb;

    void Start()
    {
        //Assign references
        rb = GetComponent<Rigidbody2D>();

        ThrowRaycast();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the stalactite needs to fall
        if (CanFall() == true && stalactiteTrapState == StalactiteTrapState.isFixed)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            stalactiteTrapState = StalactiteTrapState.isFalling;
        }

        //SECOND PHASE
        if (stalactiteTrapState == StalactiteTrapState.isAnchored)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void FixedUpdate()
    {
        //FIRST PHASE
        if (stalactiteTrapState == StalactiteTrapState.isFalling)
        {
            rb.velocity = Vector2.down * fallSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Terrain") && stalactiteTrapState == StalactiteTrapState.isFalling)
        {
            stalactiteTrapState = StalactiteTrapState.isAnchored;
        }
    }

    private void ThrowRaycast()
    {
        // Throw a Raycast below with a infinite direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity);

        if (hit.collider != null)
        {
            // Verify if the collide object has the terrain tag
            if (hit.collider.CompareTag("Terrain"))
            {
                minYPlayerPositionToFall = hit.collider.transform.position.y;
            }
        }
    }

    private bool CanFall()
    {
        playerPosition = GameManager.instance.playerPosition;

        if (playerPosition.y >= minYPlayerPositionToFall && playerPosition.x == transform.position.x)
        {
            return true;
        }

        else return false;
    }
}

