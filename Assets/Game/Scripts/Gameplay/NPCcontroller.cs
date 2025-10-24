using UnityEngine;

public class NPCcontroller :  InteractableObject
{
    //[SerializeField] private DialogueObject interactionDialogue;

    [SerializeField] private DialogueObject interactionDialogue;
    [SerializeField] private DialogueObject proximityDialogue;

    override public void  Trigger()
    {
        MessageManager.Instance.dialogueActivator.UpdateDialogueObject(interactionDialogue);

        MessageManager.Instance.Interact();

        Debug.Log("trigger");
    }

    override public void AutoTrigger()
    {
        MessageManager.Instance.dialogueActivator.UpdateDialogueObject(proximityDialogue);

        MessageManager.Instance.Interact();
    }

}
