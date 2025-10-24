using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField]
    private RectTransform responseBox;
    [SerializeField]
    private RectTransform responseButtonTemplate;
    [SerializeField]
    private RectTransform responseContainer;

    private DialogueUI dialogueUI;
    private TypewritterEffect typewritterEffect;
    private ResponseEvent[] responseEvents;
    private int[] originalResponseIndices;

    List<GameObject> tempResponseButtons = new List<GameObject>();

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>();
        typewritterEffect = GetComponent<TypewritterEffect>();
    }

    public void AddResponseEvents(ResponseEvent[] responseEventsVar)
    {
        this.responseEvents = responseEventsVar;
    }

    public void ShowResponses(Response[] responses, int[] originalIndices = null)
    {
        originalResponseIndices = originalIndices;
    
        for(int i = 0; i < responses.Length; i++)
        {
            Response response = responses[i];
            int responseIndex = i;
            
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            GameObject textResponse = responseButton.transform.GetChild(0).gameObject;
            responseButton.gameObject.SetActive(true);
            textResponse.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));

            tempResponseButtons.Add(responseButton);
            
            if (responseIndex == 0)
            {
                EventSystem.current.SetSelectedGameObject(responseButton.gameObject);
            }
        }
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response, int responseIndex)
    {
        responseBox.gameObject.SetActive(false);

        foreach(GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }
        tempResponseButtons.Clear();

        // Usar el índice original si existe
        int actualIndex = originalResponseIndices != null ? originalResponseIndices[responseIndex] : responseIndex;

        if(responseEvents != null && actualIndex < responseEvents.Length)
        {
            responseEvents[actualIndex].Invoke();
        }

        responseEvents = null;
        originalResponseIndices = null;

        // Detener efectos antes de cambiar de diálogo
        if (typewritterEffect)
            typewritterEffect.StopEffects();

        if (response.DialogueObject)
        {
            DialogueActivator dialogueActivator = MessageManager.Instance.dialogueActivator;
            dialogueActivator.UpdateDialogueObject(response.DialogueObject);
            InventoryDialogueLinker linker = dialogueActivator.GetInventoryDialogueLinker();
            DialogueResponseEvents dialogueResponseEvents = dialogueActivator.GetComponent<DialogueResponseEvents>();
            if (dialogueResponseEvents)
            {
                DialogueResponseEventType[] drEvents = dialogueResponseEvents.DREvents.ToArray();
                ResponseEvent endEvent = null;
                
                foreach (DialogueResponseEventType responseEventsVar in drEvents)
                {
                    if(responseEventsVar.DialogueObject == response.DialogueObject)
                    {
                        MessageManager.Instance.DialogueUI.AddResponseEvents(responseEventsVar.Events);
                        endEvent = responseEventsVar.Events[responseEventsVar.Events.Length - 1];
                        break;
                    }
                }
                dialogueUI.ShowDialogue(response.DialogueObject, linker, endEvent);
            }
        }
        else
        {
            dialogueUI.CloseDialogueBox();
        }
    }
}