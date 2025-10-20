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

    public void ShowResponses(Response[] responses)
    {
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

        if(responseEvents != null && responseIndex <= responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }

        responseEvents = null;

        // Detener efectos antes de cambiar de diálogo
        if (typewritterEffect)
            typewritterEffect.StopEffects();

        if (response.DialogueObject)
        {
            dialogueUI.ShowDialogue(response.DialogueObject);
        }
        else
        {
            dialogueUI.CloseDialogueBox();
        }
    }
}