using UnityEngine;

public class DoorController : InteractableObject
{
    private BoxCollider2D door;
    private BoxCollider2D zone;

    private void Awake()
    {
        door = transform.GetChild(0).GetComponent<BoxCollider2D>();
        zone = transform.GetChild(1).GetComponent<BoxCollider2D>();
        exclamation = transform.GetChild(2).gameObject;
    }

    override public void Trigger()
    {
        door.enabled=false;     //ahora mismo la puerta es de un solo uso
        zone.enabled=false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color= Color.green;
        //animacion de abrir puerta
    }
}
