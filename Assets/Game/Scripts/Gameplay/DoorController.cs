using UnityEngine;

public class DoorController : InteractableObject
{
    [SerializeField] private BoxCollider2D door;
    [SerializeField] BoxCollider2D zone;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponentInChildren<AudioSource>();
    }

    override public void Trigger()
    {
        door.enabled=false;     //ahora mismo la puerta es de un solo uso
        zone.enabled=false;
        door.GetComponent<SpriteRenderer>().color= Color.green;
        SoundManager.PlaySound(SoundType.FENCEOPEN,AudioSourceName.Main_Camera,0.8f,true,false,0.7f,1f);
        //animacion de abrir puerta
    }

    override public void AutoTrigger()
    {

    }

    //door controller esta anticuado
}
