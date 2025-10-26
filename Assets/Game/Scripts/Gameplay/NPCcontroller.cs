using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class NPCcontroller :  InteractableObject
{
    [SerializeField] private List<DialogueObject> interactionDialogues;
    [SerializeField] private List<DialogueObject> proximityDialogues;
    InventoryDialogueLinker inventoryDialogueLinker;


    public override void Trigger()
    {
        if (!inventoryDialogueLinker)
        {
            inventoryDialogueLinker = MessageManager.Instance.GetComponent<InventoryDialogueLinker>();
        }
        if (interactionDialogues.Count <= 0) return;
        if (!inventoryDialogueLinker) return;

        //InitiateDialogueProtocol();

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
        if (!inventoryDialogueLinker)
        {
            inventoryDialogueLinker = MessageManager.Instance.GetComponent<InventoryDialogueLinker>();
        }
        if (proximityDialogues.Count <= 0) return;
        if (!inventoryDialogueLinker) return;

        //InitiateDialogueProtocol();

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
