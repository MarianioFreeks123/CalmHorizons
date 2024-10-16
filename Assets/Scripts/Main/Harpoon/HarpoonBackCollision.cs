using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonBackCollision : MonoBehaviour
{
    [Header("REFERENCES IN PROYECT")]
    [SerializeField] private GameObject fakeHarpoon;
    [SerializeField] private HarpoonBehaviour harpoonBehaviour;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HarpoonBehaviour destroyedHarpoonBehaviour = collision.transform.GetComponent<HarpoonBehaviour>();

        if (harpoonBehaviour.isHarpoonAnchored() && collision.transform.CompareTag("Harpoon"))
        {
            HarpoonManager.instance.GenerateFakeHarpoon(collision, destroyedHarpoonBehaviour, transform);
        }        
    }
}
