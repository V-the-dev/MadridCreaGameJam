using UnityEngine;

public class DoorController : MonoBehaviour , IInteractuable
{
    private BoxCollider2D door;
    private BoxCollider2D zone;
    private GameObject exclamation;
    private void Awake()
    {
        door = transform.GetChild(0).GetComponent<BoxCollider2D>();
        zone = transform.GetChild(1).GetComponent<BoxCollider2D>();
        exclamation = transform.GetChild(2).gameObject;
    }

    public void NearestIndicator(bool activate)
    {
        exclamation.SetActive(activate);
    }

    public void Interact(MessageManager messageManager)
    {
        door.enabled=false;     //ahora mismo la puerta es de un solo uso
        zone.enabled=false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color= Color.green;
        //animacion de abrir puerta
    }
}
