using System;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class TwoWaySprite : MonoBehaviour
{
    private bool ZchangeOccured = false;

    private bool playerIn = false;
    
    private void OnTriggerExit2D(Collider2D other)
    {
        playerIn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(playerIn)
            return;
        
        if (!other.CompareTag("Player")) return;

        Transform playerTransform = other.transform;

        // Get the object's current Z
        float objectZ = transform.position.z;

        // If player is currently behind, move in front; otherwise move behind
        float newZ = ZchangeOccured ? objectZ - 1f : objectZ + 1f;

        // Apply the new Z to the player
        playerTransform.position = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y,
            newZ
        );

        playerIn = true;
        
        // Toggle for next time
        ZchangeOccured = !ZchangeOccured;
    }
}
