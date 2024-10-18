using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeHarpoonBehaviour : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField, Range (1f, 10f)] private float minHorizontalForce;
    [SerializeField, Range (1f, 10f)] private float maxHorizontalForce;

    [SerializeField, Range(0f, 1f)] private float substractBounceFriction;

    [SerializeField] private float minVerticalForce;
    [SerializeField] private float maxVerticalForce;

    [SerializeField] private float minRotation = -360f;
    [SerializeField] private float maxRotation = 360f;

    [SerializeField] private float timeForDissapear;
    [SerializeField] private float timeForEnableCollision;
    [SerializeField] private float fadeDuration;    

    [Header("REFERENCES IN SCENE")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("CHECKERS")]
    public float bounceDirection;

    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rb;

    void Start()
    {
        //Assign references
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();

        //Warning debug void
        if (_rb == null) DebugManager.instance.DebugErrorController(_rb.GetType().Name);
        if (_boxCollider == null) DebugManager.instance.DebugErrorController(_boxCollider.GetType().Name);

        ApplyImpulse();
        ApplyRandomRotation();
    }

    private void ApplyImpulse()
    {
        //Decide the strenght of the intial force and the oposite direction of the collision
        float randomHorizontalForce = Random.Range(minHorizontalForce, maxHorizontalForce);
        randomHorizontalForce *= bounceDirection;

        float randomVerticalForce = Random.Range(minVerticalForce, maxVerticalForce);

        Vector2 impulse = new Vector2(randomHorizontalForce, randomVerticalForce);

        _rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    private void ApplyRandomRotation()
    {
        float randomRotation = Random.Range(minRotation, maxRotation);

        transform.Rotate(0, 0, randomRotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Terrain"))
        {
            _rb.bodyType = RigidbodyType2D.Static;
            StartCoroutine(StartFadeOut());
        }
    }

    //Speed reduction
    private void OnCollisionEnter2D(Collision2D collision) => _rb.velocity = _rb.velocity* substractBounceFriction;

    IEnumerator StartFadeOut()
    {
        yield return new WaitForSeconds(timeForDissapear);

        while (spriteRenderer.color.a > 0)
        {
            Color color = spriteRenderer.color;
            color.a -= Time.deltaTime / fadeDuration;
            spriteRenderer.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator EnableCollision()
    {
        yield return new WaitForSeconds(timeForEnableCollision);
        _boxCollider.enabled = true;
    }        
}
