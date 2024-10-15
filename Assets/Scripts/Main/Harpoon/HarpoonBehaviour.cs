using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonBehaviour : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] private float harpoonSpeed;
    [SerializeField] private float verticalCollisionMargin;
    [SerializeField] private float backCollisionGap;

    [Header("CHECKERS")]
    [SerializeField] private bool hasCollidedWithTerrain;
    public int harpoonIndexInTheManager;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject backCollision;

    private Rigidbody2D _rb;
    private PlayerMovement playerMovement;
    private BoxCollider2D _boxCollider;
    private Transform playerTransform;
    private Transform playerFoots;

    private Vector2 direction;

    void Start()
    {
        //Assign references
        playerFoots = GameObject.Find("*NC*_Foots").transform;
        playerTransform = GameObject.Find("*NC*_Player").transform;

        playerMovement = playerTransform.GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();

        //Warning debug void
        if (playerFoots == null) DebugManager.instance.DebugErrorController(playerFoots.GetType().Name);
        if (playerTransform == null) DebugManager.instance.DebugErrorController(playerTransform.GetType().Name);
        if (playerMovement == null) DebugManager.instance.DebugErrorController(playerMovement.GetType().Name);
        if (_rb == null) DebugManager.instance.DebugErrorController(_rb.GetType().Name);
        if (_boxCollider == null) DebugManager.instance.DebugErrorController(_boxCollider.GetType().Name);

        //Decide direction depending of the player looking direction
        direction = playerMovement.playerIsLookingLeft ? Vector2.left : Vector2.right;

        //Put the collision in different sides of the harpoon depending of the direction
        Vector3 backCollisionLocation = backCollision.transform.position;

        if (direction == Vector2.left) 
            backCollisionLocation = new Vector3 (backCollisionLocation.x + backCollisionGap, backCollisionLocation.y, backCollisionLocation.z);
        else 
            backCollisionLocation = new Vector3(backCollisionLocation.x - backCollisionGap, backCollisionLocation.y, backCollisionLocation.z);
        
        backCollision.transform.position = backCollisionLocation;

        //Assign the position of the harpoon in the harpoon manager index
        harpoonIndexInTheManager = HarpoonManager.instance.nextHarpoonForShoot - 1;

        //Subscribe to player bend event
        playerMovement.Bend += PlayerIsBending;
    }

    private void Update()
    {
        //Disable collision when player is BELOW the harpoon
        if (hasCollidedWithTerrain)
        {
            if (playerFoots.position.y <= this.transform.position.y + verticalCollisionMargin) SetCollisionType(false);
            else if (playerFoots.position.y > this.transform.position.y - verticalCollisionMargin) SetCollisionType(true);
        }        
    }

    void FixedUpdate()
    {
        //Move if not collides with a wall
        if(!hasCollidedWithTerrain) _rb.velocity = new Vector2 (direction.x * harpoonSpeed, 0);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //COLLISION WITH TERRAIN
        //Change the behaviour of the harpoon to one anchored
        if (collision.transform.CompareTag("Terrain"))
        {
            hasCollidedWithTerrain = true;
            _rb.bodyType = RigidbodyType2D.Static;
            this.gameObject.layer = LayerMask.NameToLayer("AnchoredHarpoon");
        }
    }

    //Void used for knowing the state of the harpoon
    public bool isHarpoonAnchored() => hasCollidedWithTerrain;

    private void PlayerIsBending() => SetCollisionType(false);

    private void SetCollisionType(bool isEnabled) => _boxCollider.enabled = isEnabled;
}
