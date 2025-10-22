using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class TypewritterEffect : MonoBehaviour
{
    [SerializeField] private float typewriterSpeed = 25f;
    [Header("Effect Settings")]
    [SerializeField] private float shakeIntensity = 2f;
    [SerializeField] private float waveIntensity = 5f;
    [SerializeField] private float waveSpeed = 3f;
    [SerializeField] private float rainbowSpeed = 2f;

    [Header("List of Punctuations")] [SerializeField]
    private List<Punctuation> punctuations = new List<Punctuation>()
    {
        new Punctuation(new List<char> {'.', '!', '?'}, 0.6f),
        new Punctuation(new List<char> {',', ';', ':'}, 0.3f)
    };

    public bool IsRunning { get; private set; }

    private Coroutine typingCoroutine;
    private Coroutine effectCoroutine;
    private TMP_Text textLabel;
    private string textToType;
    private ParsedText parsedText;
    private float currentSpeed;
    private bool effectsActive;
    
    // Almacenar posiciones originales de vértices
    private Vector3[][] originalVertices;

    public void Run(ParsedText parsedTextVar, TMP_Text textLabelVar)
    {
        this.parsedText = parsedTextVar;
        this.textToType = parsedText.cleanText;
        this.textLabel = textLabelVar;
        this.currentSpeed = typewriterSpeed;

        // Detener efectos previos si existen
        StopEffects();
        
        typingCoroutine = StartCoroutine(TypeText());
        StartEffects();
    }

    public void Stop()
    {
        if (!IsRunning) return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        
        OnTypingCompleted();
    }
    
    private void StartEffects()
    {
        effectsActive = true;
        if (effectCoroutine != null)
            StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(ApplyEffects());
    }
    
    public void StopEffects()
    {
        effectsActive = false;
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }
    }

    private IEnumerator TypeText()
    {
        IsRunning = true;

        textLabel.text = textToType;
        textLabel.maxVisibleCharacters = 0;
        textLabel.ForceMeshUpdate();

        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            // Obtener velocidad modificada para este carácter
            float speedMultiplier = GetSpeedMultiplier(charIndex);
            float delayForThisChar = (1f / currentSpeed) / speedMultiplier;

            yield return new WaitForSecondsRealtime(delayForThisChar);

            charIndex++;
            textLabel.maxVisibleCharacters = charIndex;

            // Verificar puntuación
            if (charIndex > 0 && charIndex < textToType.Length)
            {
                char currentChar = textToType[charIndex - 1];
                if (IsPunctuation(currentChar, out float waitTime))
                {
                    char nextChar = textToType[charIndex];
                    if (!IsPunctuation(nextChar, out _))
                    {
                        yield return new WaitForSecondsRealtime(waitTime);
                    }
                }
            }
        }

        OnTypingCompleted();
    }

    private IEnumerator ApplyEffects()
    {
        while (effectsActive)
        {
            if (parsedText != null && parsedText.effects != null && parsedText.effects.Count > 0)
            {
                textLabel.ForceMeshUpdate();
                TMP_TextInfo textInfo = textLabel.textInfo;
                
                // Guardar posiciones originales de vértices una vez por frame
                if (originalVertices == null || originalVertices.Length != textInfo.meshInfo.Length)
                {
                    originalVertices = new Vector3[textInfo.meshInfo.Length][];
                }
                
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    var meshInfo = textInfo.meshInfo[i];
                    if (originalVertices[i] == null || originalVertices[i].Length != meshInfo.vertices.Length)
                    {
                        originalVertices[i] = new Vector3[meshInfo.vertices.Length];
                    }
                    meshInfo.vertices.CopyTo(originalVertices[i], 0);
                }
                
                for (int i = 0; i < parsedText.effects.Count; i++)
                {
                    TextEffect effect = parsedText.effects[i];
                    
                    switch (effect.type)
                    {
                        case TextEffectType.Shake:
                            ApplyShakeEffect(textInfo, effect);
                            break;
                        case TextEffectType.Wave:
                            ApplyWaveEffect(textInfo, effect);
                            break;
                        case TextEffectType.Rainbow:
                            ApplyRainbowEffect(textInfo, effect);
                            break;
                        case TextEffectType.Size:
                            ApplySizeEffect(textInfo, effect);
                            break;
                        case TextEffectType.Color:
                            ApplyColorEffect(textInfo, effect);
                            break;
                    }
                }
                
                textLabel.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
            
            yield return null;
        }
    }

    private void ApplyShakeEffect(TMP_TextInfo textInfo, TextEffect effect)
    {
        for (int i = effect.startIndex; i < effect.endIndex && i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 offset = new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0
            );

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }
    }

    private void ApplyWaveEffect(TMP_TextInfo textInfo, TextEffect effect)
    {
        for (int i = effect.startIndex; i < effect.endIndex && i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float offsetY = Mathf.Sin((Time.unscaledTime * waveSpeed) + i * 0.5f) * waveIntensity;
            Vector3 offset = new Vector3(0, offsetY, 0);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }
    }

    private void ApplyRainbowEffect(TMP_TextInfo textInfo, TextEffect effect)
    {
        for (int i = effect.startIndex; i < effect.endIndex && i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

            float hue = (Time.unscaledTime * rainbowSpeed + i * 0.1f) % 1f;
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);

            colors[vertexIndex + 0] = rainbowColor;
            colors[vertexIndex + 1] = rainbowColor;
            colors[vertexIndex + 2] = rainbowColor;
            colors[vertexIndex + 3] = rainbowColor;
        }
    }

    private void ApplySizeEffect(TMP_TextInfo textInfo, TextEffect effect)
    {
        if (!float.TryParse(effect.parameter, out float sizeMultiplier))
            sizeMultiplier = 1.5f;

        // Calcular el desplazamiento acumulado
        float cumulativeOffset = 0f;

        for (int i = effect.startIndex; i < effect.endIndex && i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            
            // Usar las posiciones originales guardadas
            Vector3[] origVertices = originalVertices[materialIndex];

            // Calcular el centro usando las posiciones originales
            Vector3 center = (origVertices[vertexIndex + 0] + origVertices[vertexIndex + 2]) / 2f;
            
            // Calcular el ancho original del carácter
            float originalWidth = origVertices[vertexIndex + 2].x - origVertices[vertexIndex + 0].x;
            float newWidth = originalWidth * sizeMultiplier;
            float widthDifference = newWidth - originalWidth;

            // Aplicar escala desde el centro usando las posiciones originales
            // y añadir el desplazamiento acumulado
            Vector3 offset = new Vector3(cumulativeOffset, 0, 0);
            
            vertices[vertexIndex + 0] = center + (origVertices[vertexIndex + 0] - center) * sizeMultiplier + offset;
            vertices[vertexIndex + 1] = center + (origVertices[vertexIndex + 1] - center) * sizeMultiplier + offset;
            vertices[vertexIndex + 2] = center + (origVertices[vertexIndex + 2] - center) * sizeMultiplier + offset;
            vertices[vertexIndex + 3] = center + (origVertices[vertexIndex + 3] - center) * sizeMultiplier + offset;
            
            // Acumular el desplazamiento para los siguientes caracteres
            cumulativeOffset += widthDifference;
        }
        
        // Aplicar el desplazamiento a todos los caracteres después del efecto
        for (int i = effect.endIndex; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            
            Vector3 offset = new Vector3(cumulativeOffset, 0, 0);
            
            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }
    }

    private void ApplyColorEffect(TMP_TextInfo textInfo, TextEffect effect)
    {
        Color color = Color.white;
        if (!string.IsNullOrEmpty(effect.parameter))
        {
            if (ColorUtility.TryParseHtmlString(effect.parameter, out Color parsedColor))
                color = parsedColor;
        }

        for (int i = effect.startIndex; i < effect.endIndex && i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

            colors[vertexIndex + 0] = color;
            colors[vertexIndex + 1] = color;
            colors[vertexIndex + 2] = color;
            colors[vertexIndex + 3] = color;
        }
    }

    private float GetSpeedMultiplier(int charIndex)
    {
        if (parsedText == null || parsedText.effects == null) return 1f;
    
        // Usar el índice del texto limpio directamente
        foreach (TextEffect effect in parsedText.effects)
        {
            if (effect.type == TextEffectType.Speed)
            {
                // Verificar si estamos en el rango del efecto
                if (charIndex >= effect.startIndex && charIndex < effect.endIndex)
                {
                    if (float.TryParse(effect.parameter, out float speedMult))
                    {
                        // Invertir el multiplicador: valores pequeños = lento, valores grandes = rápido
                        return speedMult;
                    }
                }
            }
        }
        return 1f;
    }

    private void OnTypingCompleted()
    {
        IsRunning = false;
        // Usar la longitud del texto que realmente se mostró (con los espacios parseados)
        textLabel.maxVisibleCharacters = textToType.Length;
        textLabel.ForceMeshUpdate();
    }

    private bool IsPunctuation(char character, out float waitTime)
    {
        foreach (Punctuation punctuationCategory in punctuations)
        {
            if (punctuationCategory.Punctuations.Contains(character))
            {
                waitTime = punctuationCategory.WaitTime;
                return true;
            }
        }

        waitTime = 0;
        return false;
    }

    [Serializable]
    private struct Punctuation
    {
        public List<char> Punctuations;
        public float WaitTime;

        public Punctuation(List<char> punctuations, float waitTime)
        {
            Punctuations = punctuations;
            WaitTime = waitTime;
        }
    }
}