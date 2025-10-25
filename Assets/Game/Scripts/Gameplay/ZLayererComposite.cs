using System;
using UnityEngine;

public enum ZTriggerPosition
{
    PLAYER_TO_FRONT,
    PLAYER_TO_BACK
}

public class ZLayererComposite : MonoBehaviour
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

    public void OnChildTriggerEnter2D(ZTriggerPosition colliderID, Collider2D other)
    {
        if(playerIn)
            return;

        Vector3 newPos;
        
        switch (colliderID)
        {
            case ZTriggerPosition.PLAYER_TO_BACK:
                
                if(actualPlayersPosition == PlayersPosition.IN_BACK)
                    return;
                
                // Mover A una capa por delante de B
                newPos = transform.position;
                newPos.z = other.gameObject.transform.position.z - 1f; // Por delante (menor Z)
                transform.position = newPos;
                
                actualPlayersPosition = PlayersPosition.IN_BACK;

                break;
            
            case ZTriggerPosition.PLAYER_TO_FRONT:
                
                if(actualPlayersPosition == PlayersPosition.IN_FRONT)
                    return;

                // Mover A una capa por delante de B
                newPos = transform.position;
                newPos.z = other.gameObject.transform.position.z + 1f; // Por delante (menor Z)
                transform.position = newPos;

                actualPlayersPosition = PlayersPosition.IN_FRONT;
                break;
        }

        playerIn = true;
    }
    
    public void OnChildTriggerExit2D(ZTriggerPosition colliderID, Collider2D other)
    {
        playerIn = false;
    }
}
