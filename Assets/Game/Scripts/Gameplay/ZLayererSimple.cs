using System;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class ZLayererSimple : MonoBehaviour
{
    private enum PlayersPosition
    {
        IN_BACK,
        IN_FRONT
    }

    private PlayersPosition actualPlayersPosition;
    
    public bool playerFirstBack;

    private bool playerIn = false;

    private void Start()
    {
        if (playerFirstBack)
            actualPlayersPosition = PlayersPosition.IN_BACK;
        else
            actualPlayersPosition = PlayersPosition.IN_FRONT;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(playerIn)
            return;

        if(!other.CompareTag("Player")) return;
        
        Vector3 newPos;
        
        switch (actualPlayersPosition)
        {
            case PlayersPosition.IN_FRONT:
                
                // Mover A una capa por delante de B
                newPos = transform.position;
                newPos.z = other.gameObject.transform.position.z - 1f; // Por delante (menor Z)
                transform.position = newPos;
                
                actualPlayersPosition = PlayersPosition.IN_BACK;

                break;
            
            case PlayersPosition.IN_BACK:

                // Mover A una capa por delante de B
                newPos = transform.position;
                newPos.z = other.gameObject.transform.position.z + 1f; // Por delante (menor Z)
                transform.position = newPos;

                actualPlayersPosition = PlayersPosition.IN_FRONT;
                break;
        }

        playerIn = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerIn = false;
    }
}
