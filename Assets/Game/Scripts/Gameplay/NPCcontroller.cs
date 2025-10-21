using UnityEngine;

public class NPCcontroller :  InteractableObject
{

    [SerializeField]
    private DialogueObject dialogueObject;

    void Awake()
    {
        exclamation = transform.GetChild(2).gameObject;
        messageManager= MessageManager.Instance;

        if(messageManager != null )
        {
            Debug.Log("Message Manager found in NPCcontroller");
        }
        else
        {
            Debug.LogError("Message Manager NOT found in NPCcontroller");
        }
    }

    override public void  Trigger()
    {
        MessageManager.Instance.dialogueActivator.UpdateDialogueObject(dialogueObject);

        MessageManager.Instance.Interact();
    }
}
