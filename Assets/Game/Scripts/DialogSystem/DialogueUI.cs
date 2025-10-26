using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueBox;
    [SerializeField]
    private GameObject nameTitleBox;
    [SerializeField]
    private GameObject characterBoxTemplate;
    [SerializeField]
    private RectTransform characterBoxIzqPosition;
    [SerializeField]
    private RectTransform characterBoxDerPosition;
    [SerializeField]
    private TMP_Text textLabel;
    [SerializeField]
    private TMP_Text nameLabel;
    [SerializeField]
    private TMP_Text titleLabel;

    [SerializeField] private GameObject flechita;

    [SerializeField] private Color obscuredPeople = new Color(0.3f, 0.3f, 0.3f, 1);
    [SerializeField] private float scaledObscuredPeople = 0.8f;
    [SerializeField] private float characterSpacing = 100;
    
    [Header("Text Background")]
    [SerializeField] private GameObject textBackground;
    [SerializeField] private Sprite backgroundWithName;
    [SerializeField] private Sprite backgroundWithoutName;
    [SerializeField] private List<GameObject> thingsToHideWhenBackgroundWithoutName = new List<GameObject>();

    private InputAction _acceptAction;
    private List<GameObject> leftCharacters = new List<GameObject>();
    private List<GameObject> rightCharacters  = new List<GameObject>();
    private List<string> characterNamesInScene  = new List<string>();
    private Dictionary<string, GameObject> characterDictionary  = new Dictionary<string, GameObject>();

    public bool IsOpen {  get; private set; }

    private ResponseHandler responseHandler;
    private TypewritterEffect typewritterEffect;
    private InlineImageHandler imageHandler;

    private void Awake()
    {
        typewritterEffect = GetComponent<TypewritterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        imageHandler = GetComponent<InlineImageHandler>();
        _acceptAction = InputSystem.actions.FindAction("UI/Submit");
        
        if (imageHandler != null)
            imageHandler.Initialize(textLabel);
        
        CloseDialogueBox();
    }

    public void ShowDialogue(DialogueObject dialogueObject, InventoryDialogueLinker linker = null, ResponseEvent endEvent = null)
    {
        IsOpen = true;
        dialogueBox.SetActive(true);
        nameTitleBox.SetActive(true);
        characterBoxIzqPosition.gameObject.SetActive(true);
        characterBoxDerPosition.gameObject.SetActive(true);
        SetCharacterSpritesInScene(dialogueObject);
    
        foreach (GameObject leftCharacter in leftCharacters)
        {
            leftCharacter.SetActive(true);
        }

        foreach (GameObject rightCharacter in rightCharacters)
        {
            rightCharacter.SetActive(true);
        }

        int index = 0;
        foreach (GameObject leftCharacter in leftCharacters)
        {
            leftCharacter.GetComponent<Image>().sprite = dialogueObject.Characters[index++];
        }

        foreach (GameObject rightCharacter in rightCharacters)
        {
            rightCharacter.GetComponent<Image>().sprite = dialogueObject.Characters[index++];
        }
    
        StartCoroutine(StepThroughDialogue(dialogueObject, linker, endEvent));
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        responseHandler.AddResponseEvents(responseEvents);
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject, InventoryDialogueLinker linker = null, ResponseEvent endEvent = null)
    {
        GameManager.Instance.PauseGame();
        
        yield return new WaitUntil(() => !_acceptAction.IsPressed());
        for(int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];
            
            // Cuando empiece el texto oculta la flechita
            flechita.SetActive(false);
            
            SetTalkingOrders(dialogueObject, i);
            SetNameAndTitle(dialogueObject, i);

            // Parsear el texto una sola vez antes del resto de la conversación
            ParsedText parsed = TextEffectParser.Parse(dialogue, imageHandler);
            
            // Procesar imágenes antes del typewriter
            if (imageHandler)
                imageHandler.ProcessImages(parsed);

            // Pasar el ParsedText al typewriter, no el texto original
            yield return RunTypingEffect(parsed);
            
            // Cuando termine el texto muestra la flechita
            flechita.SetActive(true);

            // - textLabel.text tiene el parsed.cleanText con los espacios
            // - maxVisibleCharacters está en parsed.cleanText.Length

            // Esto ha sido comentado para que antes de las respuestas aparezca la flechita
            // if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses)
            //     break;

            yield return null;
            yield return new WaitUntil(() => _acceptAction.WasReleasedThisFrame());
            
            // Detener efectos antes de la siguiente línea
            typewritterEffect.StopEffects();
        }

        
        if (dialogueObject.HasResponses)
        {
            // Cuando termine el texto muestra la flechita
            flechita.SetActive(false);
            
            // Filtrar respuestas según eventos
            List<Response> validResponses = new List<Response>();
            List<int> validResponseIndices = new List<int>();
            
            for (int i = 0; i < dialogueObject.Responses.Length; i++)
            {
                bool shouldShowResponse = true;
                
                if (linker)
                {
                    List<Evento> eventos = linker.TryGetEventosFromResponse(dialogueObject, i);
                    List<EventData> eventValues = linker.TryGetEventDataFromResponse(dialogueObject, i);
                    List<Objeto> objetos = linker.TryGetObjetosFromResponse(dialogueObject, i);
                    List<ObjectData> objectQuantitys = linker.TryGetObjectDataFromResponse(dialogueObject, i);
                    
                    
                    if (eventos != null)
                    {
                        for (int j = 0; j < eventos.Count; j++)
                        {
                            if (eventos[j] != null && eventValues != null)
                            {
                                shouldShowResponse = InventoryManager.Instance.GetEventValue(eventos[j].nombre) ==
                                                     eventValues[j].eventFlag;
                            }
                        }
                    }
                    
                    if (objetos != null)
                    {
                        for (int j = 0; j < objetos.Count; j++)
                        {
                            if (objetos[j] != null && objectQuantitys != null && shouldShowResponse)
                            {
                                shouldShowResponse = InventoryManager.Instance.GetItemValue(objetos[j].nombre) >= objectQuantitys[j].quantity;
                            }
                        }
                    }
                }
                
                if (shouldShowResponse)
                {
                    validResponses.Add(dialogueObject.Responses[i]);
                    validResponseIndices.Add(i);
                }
            }
            
            responseHandler.ShowResponses(validResponses.ToArray(), validResponseIndices.ToArray());
            if (endEvent != null)
            {
                
                endEvent.Invoke();
            }
            
            if (dialogueObject.MainCharacterWhenResponses)
            {
                FocusOnCharacter(dialogueObject, dialogueObject.mainCharacterIndex);
            }
        }
        else
        {
            // Detener efectos al cerrar el diálogo
            typewritterEffect.StopEffects();
            if (endEvent != null) endEvent.Invoke();
            
            // Limpiar imágenes antes de la siguiente línea
            if (imageHandler)
                imageHandler.ClearImages();
            
            CloseDialogueBox();
            GameManager.Instance.ResumeGame();
        }
    }

    private IEnumerator RunTypingEffect(ParsedText parsed)
    {
        typewritterEffect.Run(parsed, textLabel);

        while (typewritterEffect.IsRunning)
        {
            yield return null;

            if (_acceptAction.WasReleasedThisFrame())
            {
                typewritterEffect.Stop();
            }
        }
    }

    public void CloseDialogueBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        nameTitleBox.SetActive(false);
        flechita.SetActive(false);
        leftCharacters.Clear();
        rightCharacters.Clear();
        characterBoxIzqPosition.gameObject.SetActive(false);
        characterBoxDerPosition.gameObject.SetActive(false);
        characterNamesInScene.Clear();
        characterDictionary.Clear();
        textLabel.text = string.Empty;
        
        if (imageHandler)
            imageHandler.ClearImages();

        //if (GameManager.Instance)
        //{
        //    GameManager.Instance.ResumeGame();
        //}
    }

    public void SetCharacterSpritesInScene(DialogueObject dialogueObject)
    {
        List<string> characterNamesListed = new List<string>(characterNamesInScene);
        
        for (int i = 0; i < dialogueObject.Characters.Length; i++)
        {
            if (characterNamesInScene.Contains(dialogueObject.charactersName[i]))
            {
                characterNamesListed.Remove(dialogueObject.charactersName[i]);
                continue;
            }
            
            if (dialogueObject.CharactersSide[i] == SpriteSide.Izquierda)
            {
                GameObject newCharacter = Instantiate(characterBoxTemplate, characterBoxIzqPosition);
                RectTransform newCharacterRect = newCharacter.GetComponent<RectTransform>();
                
                Vector2 position = Vector2.zero;
                position.x = characterSpacing * leftCharacters.Count;
                newCharacterRect.anchoredPosition = position;
                
                leftCharacters.Add(newCharacter);
                characterNamesInScene.Add(dialogueObject.charactersName[i]);
                characterDictionary.Add(dialogueObject.charactersName[i], newCharacter);
                characterNamesListed.Remove(dialogueObject.charactersName[i]);
            }
            else
            {
                GameObject newCharacter = Instantiate(characterBoxTemplate, characterBoxDerPosition);
                RectTransform newCharacterRect = newCharacter.GetComponent<RectTransform>();
                
                Vector2 position = Vector2.zero;
                position.x = characterSpacing * rightCharacters.Count;
                newCharacterRect.anchoredPosition = position;
                
                rightCharacters.Add(newCharacter);
                characterNamesInScene.Add(dialogueObject.charactersName[i]);
                characterDictionary.Add(dialogueObject.charactersName[i], newCharacter);
                characterNamesListed.Remove(dialogueObject.charactersName[i]);
            }
        }

        while (characterNamesListed.Count > 0)
        {
            string characterNameToRemove = characterNamesListed[0];
            GameObject characterToErase = characterDictionary[characterNameToRemove];
            float positionXCharacterToErase = characterToErase.GetComponent<RectTransform>().anchoredPosition.x;
            
            if (leftCharacters.Contains(characterToErase))
            {
                leftCharacters.Remove(characterToErase);
                
                foreach (GameObject leftCharacter in leftCharacters)
                {
                    RectTransform tempRectTransform = leftCharacter.GetComponent<RectTransform>();
                    if (tempRectTransform.anchoredPosition.x > positionXCharacterToErase)
                    {
                        tempRectTransform.anchoredPosition = new Vector2(
                            tempRectTransform.anchoredPosition.x - characterSpacing, 
                            tempRectTransform.anchoredPosition.y
                        );
                    }
                }
            }
            else if (rightCharacters.Contains(characterToErase))
            {
                rightCharacters.Remove(characterToErase);
                
                foreach (GameObject rightCharacter in rightCharacters)
                {
                    RectTransform tempRectTransform = rightCharacter.GetComponent<RectTransform>();
                    if (tempRectTransform.anchoredPosition.x > positionXCharacterToErase)
                    {
                        tempRectTransform.anchoredPosition = new Vector2(
                            tempRectTransform.anchoredPosition.x - characterSpacing, 
                            tempRectTransform.anchoredPosition.y
                        );
                    }
                }
            }
            
            characterDictionary.Remove(characterNameToRemove);
            characterNamesInScene.Remove(characterNameToRemove);
            characterNamesListed.RemoveAt(0);
            
            Destroy(characterToErase);
        }
    }

    public void FocusOnCharacter(DialogueObject dialogueObject, int characterIndex)
    {
        string talkingPersonName = dialogueObject.charactersName[characterIndex];
        
        GameObject talkingPerson = null;
        
        foreach (GameObject leftCharacter in leftCharacters)
        {
            if (characterDictionary[talkingPersonName] == leftCharacter)
            {
                talkingPerson = leftCharacter;
            }
            else
            {
                leftCharacter.GetComponent<Image>().color = obscuredPeople;
                leftCharacter.GetComponent<RectTransform>().localScale = new Vector3(scaledObscuredPeople, scaledObscuredPeople, scaledObscuredPeople);
            }
        }
        
        foreach (GameObject rightCharacter in rightCharacters)
        {
            if (characterDictionary[talkingPersonName] == rightCharacter)
            {
                talkingPerson = rightCharacter;
            }
            else
            {
                rightCharacter.GetComponent<Image>().color = obscuredPeople;
                rightCharacter.GetComponent<RectTransform>().localScale = new Vector3(scaledObscuredPeople,
                    scaledObscuredPeople, scaledObscuredPeople);
            }
        }

        if (talkingPerson)
        {
            talkingPerson.GetComponent<Image>().color = Color.white;
            talkingPerson.transform.SetAsLastSibling();
            talkingPerson.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }
    
    public void SetTalkingOrders(DialogueObject dialogueObject, int dialogueIndex)
    {
        int characterIndex = dialogueObject.SpriteIndexes[dialogueIndex];
        FocusOnCharacter(dialogueObject, characterIndex);
    }

    public void SetNameAndTitle(DialogueObject dialogueObject, int dialogueIndex)
    {
        nameLabel.text = dialogueObject.charactersName[dialogueObject.SpriteIndexes[dialogueIndex]];
        titleLabel.text = dialogueObject.charactersTitle[dialogueObject.SpriteIndexes[dialogueIndex]];
        if (nameLabel.text  == "" && titleLabel.text == "")
        {
            textBackground.GetComponent<Image>().sprite = backgroundWithoutName;
            foreach (GameObject character in leftCharacters)
            {
                Destroy(character);
            }

            foreach (GameObject character in rightCharacters)
            {
                Destroy(character);
            }
            leftCharacters.Clear();
            rightCharacters.Clear();
            characterNamesInScene.Clear();
            foreach (GameObject thing in thingsToHideWhenBackgroundWithoutName)
            {
                thing.SetActive(false);
            }
        }
        else
        {
            textBackground.GetComponent<Image>().sprite = backgroundWithName;
            foreach (GameObject thing in thingsToHideWhenBackgroundWithoutName)
            {
                thing.SetActive(true);
            }
        }
    }
}