using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] float timeBeforeDestroy;

    private void Start() => StartCoroutine(DestroyObj());
    private IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(timeBeforeDestroy);
        Destroy(gameObject);
    }
}
