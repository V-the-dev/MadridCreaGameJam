using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractuable
{
    [SerializeField]
    private DialogueObject dialogueObject;

    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
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

        messageManager.DialogueUI.ShowDialogue(dialogueObject);
    }

    public void NearestIndicator(bool activate)
    {
        // Implement visual indicator logic here if needed
    }
}
