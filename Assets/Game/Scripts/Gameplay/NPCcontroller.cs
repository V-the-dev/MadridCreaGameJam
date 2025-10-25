using System.Collections.Generic;
using UnityEngine;

public class NPCcontroller :  InteractableObject
{
    [SerializeField] private List<DialogueObject> interactionDialogues;
    [SerializeField] private List<DialogueObject> proximityDialogues;
    InventoryDialogueLinker inventoryDialogueLinker;

    void Awake()
    {
        inventoryDialogueLinker = MessageManager.Instance.GetComponent<InventoryDialogueLinker>();
        //if(transform.GetChild)
    }

    public override void Trigger()
    {
        if (interactionDialogues.Count <= 0) return;
        if (!inventoryDialogueLinker) return;
        foreach (DialogueObject interactionDialogue in interactionDialogues)
        {
            if (inventoryDialogueLinker.CanShowDialogue(interactionDialogue) == true)
            {
                MessageManager.Instance.dialogueActivator.UpdateDialogueObject(interactionDialogue);
                MessageManager.Instance.Interact();
                break;
            }
        }
    }

    public override void AutoTrigger()
    {
        if (proximityDialogues.Count <= 0) return;
        if (!inventoryDialogueLinker) return;
        foreach (DialogueObject proximityDialogue in proximityDialogues)
        {
            if (inventoryDialogueLinker.CanShowDialogue(proximityDialogue) == true)
            {
                MessageManager.Instance.dialogueActivator.UpdateDialogueObject(proximityDialogue);
                MessageManager.Instance.Interact();
                break;
            }
        }
    }



}
