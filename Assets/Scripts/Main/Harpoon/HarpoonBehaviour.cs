using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonBehaviour : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] private float harpoonSpeed;

    [Header("CHECKERS")]
    [SerializeField] bool hasCollided;

    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private BoxCollider boxCollider;
    
    private Vector2 direction;

    void Start()
    {
        //Assign references
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GameObject.Find("*NC*_Player").GetComponent<PlayerMovement>();

        //Decide direction depending of the player looking direction
        direction = playerMovement.playerIsLookingLeft ? new Vector2(-1, 0) : new Vector2(1, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move if not collides with a wall
        if(!hasCollided) rb.velocity = new Vector2 (direction.x * harpoonSpeed, 0);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Change the behaviour of the harpoon to one anchored
        if (collision.transform.CompareTag("Terrain"))
        {
            hasCollided = true;
            rb.bodyType = RigidbodyType2D.Static;
            this.gameObject.layer = LayerMask.NameToLayer("AnchoredHarpoon");
        }

        //Destroy the anchored harpoon who collide (Only if its moving)
        if (collision.transform.CompareTag("Harpoon") && !hasCollided)
        {
            if (collision.transform.GetComponent<HarpoonBehaviour>().isHarpoonAnchored())
            {
                Destroy(collision.transform);
            }
        }
    }

    //Void used for knowing the state of the harpoon
    public bool isHarpoonAnchored()
    {
        return hasCollided;
    }
}
