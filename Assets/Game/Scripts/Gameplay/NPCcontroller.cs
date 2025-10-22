using UnityEngine;

public class NPCcontroller :  InteractableObject
{

    [SerializeField]
    private DialogueObject dialogueObject;

    void Awake()
    {
        exclamation = transform.GetChild(2).gameObject;
        messageManager= MessageManager.Instance;
    }

    override public void  Trigger()
    {
        MessageManager.Instance.dialogueActivator.UpdateDialogueObject(dialogueObject);

        MessageManager.Instance.Interact();
    }

}
