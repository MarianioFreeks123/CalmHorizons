using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StalactiteTrapDetector;

public class StalactiteBeak : MonoBehaviour
{
    [Header("REFERENCES IN SCENE")]
    [SerializeField] StalactiteTrapDetector detector;
    [SerializeField] Rigidbody2D platformRB;
    [SerializeField] MMF_Player collisionFeedback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Terrain") && detector.stalactiteTrapState == StalactiteTrapDetector.StalactiteTrapState.isFalling)
        {
            detector.stalactiteTrapState = StalactiteTrapState.isAnchored;
            platformRB.bodyType = RigidbodyType2D.Static;
            collisionFeedback.PlayFeedbacks();
        }
    }
}
