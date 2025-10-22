using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractuable
{
    [SerializeField]
    private DialogueObject dialogueObject;

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    public InventoryDialogueLinker GetInventoryDialogueLinker()
    {
        return GetComponent<InventoryDialogueLinker>();
    }

    public void Interact(MessageManager messageManager)
    {
        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if(responseEvents.DialogueObject == dialogueObject)
            {
                messageManager.DialogueUI.AddResponseEvents(responseEvents.Events);
                break;
            }
        }

        InventoryDialogueLinker linker = GetComponent<InventoryDialogueLinker>();
        messageManager.DialogueUI.ShowDialogue(dialogueObject, linker);
    }

    public void NearestIndicator(bool activate)
    {
        // Implement visual indicator logic here if needed
    }
}
