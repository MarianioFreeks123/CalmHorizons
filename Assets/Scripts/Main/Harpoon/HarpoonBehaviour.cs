using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonBehaviour : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] private float harpoonSpeed;
    [SerializeField] private float bounceMomentumDuration;
    [SerializeField] private float bounceForce;

    [Header("CHECKERS")]
    [SerializeField] private bool hasCollidedWithTerrain;
    [SerializeField] private bool isInBounceMomentum = false;
    public int harpoonIndexInTheManager;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private Rigidbody2D _rb;
    private PlayerMovement playerMovement;

    private Vector2 direction;

    void Start()
    {
        //Assign references
        _rb = GetComponent<Rigidbody2D>();
        playerMovement = GameObject.Find("*NC*_Player").GetComponent<PlayerMovement>();

        //Decide direction depending of the player looking direction
        direction = playerMovement.playerIsLookingLeft ? new Vector2(-1, 0) : new Vector2(1, 0);

        //Assign the position of the harpoon in the harpoon manager index
        harpoonIndexInTheManager = HarpoonManager.instance.nextHarpoonForShoot - 1;

        //DEBUG [CAN BE DELEATED]
        spriteRenderer.color = new Color(Random.value, Random.value, Random.value);
    }

    private void Update()
    {
        //Destroy the harpoon when cross camera limits
        DestroyHarpoonIfCrossCameraLimits();
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

        //COLLISION WITH OTHER HARPOONS
        //Destroy the anchored harpoon who collide (Only if its moving)
        if (collision.transform.CompareTag("Harpoon") && !hasCollidedWithTerrain)
        {
            HarpoonBehaviour destroyedHarpoonBehaviour = collision.transform.GetComponent<HarpoonBehaviour>();

            if (destroyedHarpoonBehaviour.isHarpoonAnchored())
            {
                Destroy(gameObject);
                HarpoonManager.instance.DestroyHarpoon(harpoonIndexInTheManager);
            }
        }

        //PLAYER BOUNCE
        if (collision.transform.CompareTag("Player") && isInBounceMomentum)
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            playerRb.velocity = Vector2.zero;
            playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
    }

    //Void used for knowing the state of the harpoon
    public bool isHarpoonAnchored()
    {
        return hasCollidedWithTerrain;
    }

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
}
