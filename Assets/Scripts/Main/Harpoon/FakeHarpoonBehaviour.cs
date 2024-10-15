using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeHarpoonBehaviour : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] private float minHorizontalForce;
    [SerializeField] private float maxHorizontalForce;

    [SerializeField] private float minVerticalForce;
    [SerializeField] private float maxVerticalForce;

    [SerializeField] private float minRotation = -360f;
    [SerializeField] private float maxRotation = 360f;

    [SerializeField] private float timeForDissapear;
    [SerializeField] private float timeForEnableCollision;
    [SerializeField] private float fadeDuration;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] private SpriteRenderer spriteRenderer;

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
        float randomHorizontalForce = Random.Range(minHorizontalForce, maxHorizontalForce);
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
