using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FeedbackPlayerController : MonoBehaviour
{
    [Header("REFERENCES IN SCENE")]
    //0-Left, 1-Right
    [SerializeField] Transform[] shootFeedbackPositions;
    [SerializeField] Transform[] walkFeedbackPositions;

    [SerializeField] Transform shootFeedbackTransform;

    [SerializeField] HarpoonLauncher harpoonLauncher;
    [SerializeField] PlayerMovement playerMovement;

    [SerializeField] MMF_Player shootFeedback;

    void Start()
    {
        //Subscribe shoot event
        harpoonLauncher.ShootHarpoon += GenerateSmokeParticles;
    }

    //Void that generate smoke particles when player shoot
    private void GenerateSmokeParticles() 
    {
        //Player have enough ammo
        if (HarpoonManager.instance.ammo > 0)
        {
            if (playerMovement.playerIsLookingLeft) shootFeedbackTransform.position = shootFeedbackPositions[0].position;
            else shootFeedbackTransform.position = shootFeedbackPositions[1].position;

            shootFeedback.PlayFeedbacks();
        }        
    }
}
