using System;
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
        bool foundResponseEvent = false;
        DialogueResponseEvents dialogueResponseEvents = GetComponent<DialogueResponseEvents>();
        if (dialogueResponseEvents)
        {
            DialogueResponseEventType[] drEvents = dialogueResponseEvents.DREvents.ToArray();
            ResponseEvent endEvent = null;
            foreach (DialogueResponseEventType responseEvents in drEvents)
            {
                if(responseEvents.DialogueObject == dialogueObject)
                {
                    messageManager.DialogueUI.AddResponseEvents(responseEvents.Events);
                    endEvent = responseEvents.Events[responseEvents.Events.Length - 1];
                    foundResponseEvent = true;
                    break;
                }
            }

            if (!foundResponseEvent)
            {
                messageManager.DialogueUI.ClearResponseEvents();
                endEvent = null;
            }

            InventoryDialogueLinker linker = GetComponent<InventoryDialogueLinker>();
            messageManager.DialogueUI.ShowDialogue(dialogueObject, linker, endEvent);
        }
    }

}
