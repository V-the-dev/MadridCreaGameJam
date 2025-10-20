using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum TextEffectType
{
    None,
    Shake,
    Wave,
    Rainbow,
    Size,
    Color,
    Speed,
    Image
}

[System.Serializable]
public class TextEffect
{
    public TextEffectType type;
    public int startIndex;
    public int endIndex;
    public string parameter;

    public TextEffect(TextEffectType type, int start, int end, string param = "")
    {
        this.type = type;
        this.startIndex = start;
        this.endIndex = end;
        this.parameter = param;
    }
}

public class TextEffectParser
{
    private static readonly Dictionary<string, TextEffectType> tagToEffect = new Dictionary<string, TextEffectType>
    {
        { "shake", TextEffectType.Shake },
        { "wave", TextEffectType.Wave },
        { "rainbow", TextEffectType.Rainbow },
        { "size", TextEffectType.Size },
        { "color", TextEffectType.Color },
        { "speed", TextEffectType.Speed },
        { "img", TextEffectType.Image }
    };

    public static ParsedText Parse(string text, InlineImageHandler imageHandler = null)
    {
        List<TextEffect> effects = new List<TextEffect>();
        string cleanText = text;
        int offset = 0;

        // Pattern para tags con o sin parámetros: <tag> o <tag=param>
        string pattern = @"<(/?)(\w+)(?:=([^>]+))?>";
        MatchCollection matches = Regex.Matches(text, pattern);

        Stack<TagInfo> tagStack = new Stack<TagInfo>();

        foreach (Match match in matches)
        {
            bool isClosing = match.Groups[1].Value == "/";
            string tagName = match.Groups[2].Value.ToLower();
            string parameter = match.Groups[3].Value;

            if (!tagToEffect.ContainsKey(tagName))
                continue;

            if (!isClosing)
            {
                // Tag de apertura
                int position = match.Index - offset;
                
                // Para imágenes, reemplazar el tag con espacios según el tamaño
                if (tagToEffect[tagName] == TextEffectType.Image)
                {
                    int numSpaces = 1; // Valor por defecto
    
                    if (imageHandler)
                    {
                        float imageWidth = imageHandler.GetImageWidth(parameter);
        
                        // Calcular el ancho de un espacio en la fuente actual
                        float spaceWidth = CalculateSpaceWidth(imageHandler);
        
                        if (spaceWidth > 0 && imageWidth > 0)
                        {
                            numSpaces = Mathf.CeilToInt(imageWidth / spaceWidth);
                        }
                    }
    
                    string spaces = new string(' ', numSpaces);
    
                    effects.Add(new TextEffect(
                        TextEffectType.Image,
                        position,
                        position + numSpaces,
                        parameter
                    ));
    
                    // Reemplazar el tag con espacios
                    cleanText = cleanText.Remove(match.Index - offset, match.Length);
                    cleanText = cleanText.Insert(match.Index - offset, spaces);
                    offset += match.Length - numSpaces;
                }
                else
                {
                    tagStack.Push(new TagInfo
                    {
                        type = tagToEffect[tagName],
                        startIndex = position,
                        parameter = parameter
                    });
                }
            }
            else
            {
                // Tag de cierre (no se aplica a imágenes)
                if (tagStack.Count > 0)
                {
                    TagInfo openTag = tagStack.Pop();
                    int position = match.Index - offset;
                    
                    effects.Add(new TextEffect(
                        openTag.type,
                        openTag.startIndex,
                        position,
                        openTag.parameter
                    ));
                }
                
                // Remover el tag del texto
                cleanText = cleanText.Remove(match.Index - offset, match.Length);
                offset += match.Length;
            }

            // Remover tags de apertura no-imagen
            if (!isClosing && tagToEffect.ContainsKey(tagName) && tagToEffect[tagName] != TextEffectType.Image)
            {
                cleanText = cleanText.Remove(match.Index - offset, match.Length);
                offset += match.Length;
            }
        }

        return new ParsedText
        {
            cleanText = cleanText,
            effects = effects
        };
    }

    private class TagInfo
    {
        public TextEffectType type;
        public int startIndex;
        public string parameter;
    }
    
    private static float CalculateSpaceWidth(InlineImageHandler imageHandler)
    {
        return imageHandler.GetSpaceWidth();
    }
}

[System.Serializable]
public class ParsedText
{
    public string cleanText;
    public List<TextEffect> effects;
}