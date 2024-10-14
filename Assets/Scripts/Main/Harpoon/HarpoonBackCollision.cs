using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonBackCollision : MonoBehaviour
{
    [Header("REFERENCES IN PROYECT")]
    [SerializeField] private GameObject fakeHarpoon;
    [SerializeField] HarpoonBehaviour harpoonBehaviour;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HarpoonBehaviour destroyedHarpoonBehaviour = collision.transform.GetComponent<HarpoonBehaviour>();
        
        if (harpoonBehaviour.isHarpoonAnchored() && collision.transform.CompareTag("Harpoon"))
        {           

            GameObject fakeHarpoonInstance = Instantiate(fakeHarpoon, this.transform.position, Quaternion.identity);

            //Assign a parent for order propouses
            fakeHarpoonInstance.transform.parent = GameObject.Find("*NC*_FakeHarpoonParent").transform;

            Destroy(collision.gameObject);
            HarpoonManager.instance.DestroyHarpoon(destroyedHarpoonBehaviour.harpoonIndexInTheManager);
        }
    }
}
