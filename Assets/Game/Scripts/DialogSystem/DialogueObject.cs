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

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    public Sprite[] characters = new Sprite[0];
    
    public string[] charactersName = new string[0];
    
    public SpriteSide[] charactersSide = new SpriteSide[0];
    
    public ConversationLine[] conversationLine = new ConversationLine[0];
    
    public Response[] responses = new Response[0];

    // Propiedades de acceso
    public string[] Dialogue => conversationLine.Select(line => line.dialogue).ToArray();
    
    public int[] SpriteIndexes => conversationLine.Select(line => line.spriteIndex).ToArray();
    
    public Sprite[] Characters => characters;
    
    public string[] CharactersName => charactersName;
    
    public SpriteSide[] CharactersSide => charactersSide;
    
    public bool HasResponses => responses != null && responses.Length > 0;
    
    public Response[] Responses => responses;
}