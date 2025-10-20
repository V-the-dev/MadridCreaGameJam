using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum SpriteSide
{
    Izquierda,
    Derecha
}

[System.Serializable]
public class ConversationLine
{
    [TextArea]
    public string dialogue;
    public int spriteIndex;
}

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Dialogo/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    public Sprite[] characters = Array.Empty<Sprite>();
    
    public string[] charactersName = Array.Empty<string>();

    public string[] charactersTitle = Array.Empty<string>();
    
    public SpriteSide[] charactersSide = Array.Empty<SpriteSide>();
    
    public ConversationLine[] conversationLine = Array.Empty<ConversationLine>();
    
    public Response[] responses = Array.Empty<Response>();

    // Propiedades de acceso
    public string[] Dialogue => conversationLine.Select(line => line.dialogue).ToArray();
    
    public int[] SpriteIndexes => conversationLine.Select(line => line.spriteIndex).ToArray();
    
    public Sprite[] Characters => characters;
    
    public string[] CharactersName => charactersName;
    
    public string[] CharactersTitle => charactersTitle;
    
    public SpriteSide[] CharactersSide => charactersSide;
    
    public bool HasResponses => responses != null && responses.Length > 0;
    
    public Response[] Responses => responses;
}