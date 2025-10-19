using UnityEngine;

public class NPCcontroller : MonoBehaviour , IInteractuable
{
    private GameObject exclamation;

    [SerializeField]
    private DialogueObject dialogueObject;

    void Awake()
    {
        exclamation = transform.GetChild(2).gameObject;
    }

    void IInteractuable.Interact(MessageManager messageManager)
    {
        messageManager.dialogueActivator.UpdateDialogueObject(dialogueObject);
        
        messageManager.Interact();
    }

    public void NearestIndicator(bool activate)
    {
        exclamation.SetActive(activate);
    }
}
