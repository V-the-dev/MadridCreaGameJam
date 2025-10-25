using System;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class TriggerRelay : MonoBehaviour
{
    public ZTriggerPosition triggerID;
    
    private ZLayererComposite parent;

    private bool playerIn;
    
    private void Start()
    {
        parent = GetComponentInParent<ZLayererComposite>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        
        parent.OnChildTriggerEnter2D(triggerID, other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        parent.OnChildTriggerExit2D(triggerID, other);

    }
}
