using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloor : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] private float icyDeceleration;
    [SerializeField] float playerGroundDeceleration;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Change to the new player's friction
        if (collision.transform.CompareTag("Player"))
        {
            ModifyPlayersFriction(collision, icyDeceleration);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Return the original player's friction
        if (collision.transform.CompareTag("Player"))
        {
            ModifyPlayersFriction(collision, playerGroundDeceleration);
        }
    }

    public void ModifyPlayersFriction(Collision2D collision, float newFriction)
    {
        PlayerMovement playerMovement = collision.transform.GetComponent<PlayerMovement>();

        if (playerMovement != null) playerMovement.groundDeceleration = newFriction;
    }
}
