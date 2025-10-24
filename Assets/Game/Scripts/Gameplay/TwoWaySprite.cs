using System;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class TwoWaySprite : MonoBehaviour
{
    private bool ZchangeOccured = false;

    public int playerInFrontFirst = 0;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (ZchangeOccured)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, +2f);
            }

            ZchangeOccured = !ZchangeOccured;
        }
    }
}
