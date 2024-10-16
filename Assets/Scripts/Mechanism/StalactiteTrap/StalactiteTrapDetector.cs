using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactiteTrapDetector : MonoBehaviour
{    
    public enum StalactiteTrapState {isFixed, isFalling, isAnchored};

    [Header("PARAMETERS")]
    [SerializeField] float gravityFallForce;
    [SerializeField] float timeBeforeFall;

    [Header("CHECKERS")]
    [SerializeField] public StalactiteTrapState stalactiteTrapState;

    [Header("REFERENCE SIN SCENE")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private MMF_Player detachmentFeedback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check if the stalactite needs to fall
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("Harpoon"))
        {
            if(stalactiteTrapState == StalactiteTrapState.isFixed) StartCoroutine(Detachment());
        }
    }

    private IEnumerator Detachment() 
    {
        detachmentFeedback.PlayFeedbacks();

        yield return new WaitForSeconds(timeBeforeFall);

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityFallForce;
        stalactiteTrapState = StalactiteTrapState.isFalling;        
    }
}

