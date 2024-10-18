using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StalactiteTrapDetector;

public class StalactitePlatform : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] private LayerMask fakeHarpoonLayerMask;

    [Header("REFERENCES IN SCENE")]
    [SerializeField] private StalactiteTrapDetector detector;

    private BoxCollider2D boxCollider2D;

    private void Start()
    {
        //Assign references
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        //Disable collisions with fake harpoons if is falling
        if (detector.stalactiteTrapState == StalactiteTrapDetector.StalactiteTrapState.isFalling) boxCollider2D.excludeLayers = fakeHarpoonLayerMask;
 
        else if (detector.stalactiteTrapState == StalactiteTrapDetector.StalactiteTrapState.isAnchored) boxCollider2D.excludeLayers &= ~fakeHarpoonLayerMask;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player")) collision.transform.SetParent(transform, true);

        else if (collision.transform.CompareTag("Harpoon"))
        {
            HarpoonBehaviour destroyedHarpoonBehaviour = collision.transform.GetComponent<HarpoonBehaviour>();

            //Get the opposite direction that the fake harpoon will choose to bounce
            float bounceDirection = -destroyedHarpoonBehaviour.direction.x;

            HarpoonManager.instance.GenerateFakeHarpoon(collision, destroyedHarpoonBehaviour, transform, bounceDirection);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Transform mainGameObj = GameObject.Find("--MAIN--").transform;
            collision.transform.SetParent(mainGameObj, true);
        }
    }
}
