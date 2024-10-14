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
    [SerializeField] private float fadeDuration;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
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

    private void OnCollisionEnter2D(Collision2D collision)
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
}
