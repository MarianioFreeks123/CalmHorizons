using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactitePlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.transform.SetParent(transform, true);
        }

        if (collision.transform.CompareTag("Harpoon"))
        {
            HarpoonBehaviour destroyedHarpoonBehaviour = collision.transform.GetComponent<HarpoonBehaviour>();
            HarpoonManager.instance.GenerateFakeHarpoon(collision, destroyedHarpoonBehaviour, transform);
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
