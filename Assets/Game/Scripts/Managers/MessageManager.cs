using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance { get; private set; }
 
    [SerializeField]
    private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;

    [HideInInspector]
    public DialogueActivator dialogueActivator = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
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
        GetComponent<DialogueActivator>().Interact(this);
    }
}
