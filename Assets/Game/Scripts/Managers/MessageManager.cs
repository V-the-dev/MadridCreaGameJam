using UnityEngine;

public class MessageManager : MonoBehaviour
{
    [SerializeField]
    private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;

    [HideInInspector]
    public DialogueActivator dialogueActivator = null;

    private void Start()
    {
        if (dialogueActivator == null)
        {
            dialogueActivator = GetComponent<DialogueActivator>();
        }
    }

    private void Update()
    {
        if (dialogueUI.IsOpen)
        {
            return;
        }
    }
    public void Interactuar()
    {
        Interact();
    }

    public void Interact()
    {
        GetComponent<DialogueActivator>().Interact(this);
    }
}
