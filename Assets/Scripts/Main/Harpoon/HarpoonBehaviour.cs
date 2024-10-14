using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonBehaviour : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] private float harpoonSpeed;
    [SerializeField] private float bounceMomentumDuration;
    [SerializeField] private float bounceForce;
    [SerializeField] private float verticalCollisionMargin;

    [Header("CHECKERS")]
    [SerializeField] private bool hasCollidedWithTerrain;
    [SerializeField] private bool isInBounceMomentum = false;
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

        //Decide direction depending of the player looking direction
        direction = playerMovement.playerIsLookingLeft ? Vector2.left : Vector2.right;

        //Put the collision in different sides of the harpoon depending of the direction
        Vector3 backCollisionLocation = backCollision.transform.position;
        if (direction == Vector2.left) backCollisionLocation = new Vector3 (backCollisionLocation.x + 0.321f, backCollisionLocation.y, backCollisionLocation.z);
        else backCollisionLocation = new Vector3(backCollisionLocation.x - 0.321f, backCollisionLocation.y, backCollisionLocation.z);
        backCollision.transform.position = backCollisionLocation;

        //Assign the position of the harpoon in the harpoon manager index
        harpoonIndexInTheManager = HarpoonManager.instance.nextHarpoonForShoot - 1;

        //DEBUG [CAN BE DELEATED]
        spriteRenderer.color = new Color(Random.value, Random.value, Random.value);
    }

    private void Update()
    {
        //Destroy the harpoon when cross camera limits
        DestroyHarpoonIfCrossCameraLimits();

        //Disable collision when player is BELOW the harpoon
        if (hasCollidedWithTerrain)
        {
            if (playerFoots.position.y <= this.transform.position.y + verticalCollisionMargin) SetCollisionType(false);
            else if (playerFoots.position.y > this.transform.position.y - verticalCollisionMargin) SetCollisionType(true);
        }        
    }

    // Update is called once per frame
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

            //Start bounce momentum
            StartCoroutine(BounceMomentum());
        }

        //PLAYER COLLISION
        if (collision.transform.CompareTag("Player"))
        {
            //Player bounce
            if (isInBounceMomentum)
            {
                playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
                if (playerMovement != null) playerMovement.Bounce(bounceForce);
            }
        }
    }

    //Void used for knowing the state of the harpoon
    public bool isHarpoonAnchored() => hasCollidedWithTerrain;

    private void DestroyHarpoonIfCrossCameraLimits()
    {
        // Obtener la cámara principal
        Camera cam = Camera.main;

        // Convertir la posición del objeto a coordenadas de la vista de la cámara
        Vector3 viewportPosition = cam.WorldToViewportPoint(transform.position);

        // Comprobar si el objeto está fuera de los límites de la cámara
        if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
        {
            Destroy(gameObject);
            HarpoonManager.instance.DestroyHarpoon(harpoonIndexInTheManager);
        }
    }
    private IEnumerator BounceMomentum()
    {
        //DEBUG [CAN BE DELEATED]
        Color previousColor = spriteRenderer.color;        
        spriteRenderer.color = Color.yellow;

        isInBounceMomentum = true;

        yield return new WaitForSeconds(bounceMomentumDuration);

        isInBounceMomentum = false;
        spriteRenderer.color = previousColor;
    }

    private void SetCollisionType(bool isEnabled) => _boxCollider.enabled = isEnabled;
}
