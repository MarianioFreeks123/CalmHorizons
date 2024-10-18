using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrittleIcyGround : IceFloor
{
    public enum IceStates { Perfect, Damage, Broken };

    [Header("CHECKERS")]
    [SerializeField] IceStates iceStates;

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
}
