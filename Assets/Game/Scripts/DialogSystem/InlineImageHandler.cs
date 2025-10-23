using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public class InlineImageData
{
    public string key;
    public Sprite sprite;
    [HideInInspector] public float sizeMultiplier = 50f;
}

public class InlineImageHandler : MonoBehaviour
{
    [SerializeField] private List<InlineImageData> imageDatabase = new List<InlineImageData>();
    [SerializeField] private GameObject imageTemplate;
    
    private TMP_Text textLabel;
    private List<GameObject> activeImages = new List<GameObject>();
    private Coroutine updateCoroutine;
    
    public void Initialize(TMP_Text textLabelVar)
    {
        textLabel = textLabelVar;
        
        if (imageTemplate != null)
            imageTemplate.SetActive(false);
    }
    
    /// <summary>
    /// Calcula el ancho que ocupará una imagen en píxeles
    /// </summary>
    public float GetImageWidth(string key)
    {
        InlineImageData imageData = GetImageDataByKey(key);
        if (imageData != null)
        {
            return imageData.sizeMultiplier;
        }
        return 0f;
    }
    
    /// <summary>
    /// Calcula el ancho del carácter espacio en la fuente actual
    /// </summary>
    public float GetSpaceWidth()
    {
        if (textLabel == null) return 10f;
    
        // Medir la diferencia entre "a a" y "aa"
        textLabel.text = "a a";
        textLabel.ForceMeshUpdate();
        float withSpace = textLabel.textInfo.characterInfo[2].origin;
    
        textLabel.text = "aa";
        textLabel.ForceMeshUpdate();
        float withoutSpace = textLabel.textInfo.characterInfo[1].origin;
    
        return withSpace - withoutSpace;
    }
    
    public void ProcessImages(ParsedText parsedText)
    {
        ClearImages();
        
        if (updateCoroutine != null)
            StopCoroutine(updateCoroutine);
        
        updateCoroutine = StartCoroutine(UpdateImagesPosition(parsedText));
    }
    
    private IEnumerator UpdateImagesPosition(ParsedText parsedText)
    {
        // Esperar un frame para asegurar que el texto se ha configurado
        yield return null;
        
        // Crear las imágenes primero
        foreach (TextEffect effect in parsedText.effects)
        {
            if (effect.type == TextEffectType.Image)
            {
                CreateInlineImage(effect);
            }
        }
        
        // Continuar actualizando las posiciones mientras haya imágenes activas
        while (activeImages.Count > 0)
        {
            UpdateAllImagePositions();
            yield return null;
        }
    }
    
    private void CreateInlineImage(TextEffect effect)
    {
        if (!imageTemplate || !textLabel)
        {
            Debug.LogWarning("Image Template o TextLabel no asignado en InlineImageHandler");
            return;
        }
            
        // Buscar el sprite en la base de datos
        InlineImageData imageData = GetImageDataByKey(effect.parameter);
        if (imageData == null || !imageData.sprite)
        {
            Debug.LogWarning($"No se encontró imagen con key: {effect.parameter}");
            return;
        }
        
        // Crear la imagen en el mismo parent que el textLabel
        GameObject imageObj = Instantiate(imageTemplate, textLabel.transform);
        imageObj.SetActive(true);
        imageObj.name = $"InlineImage_{effect.parameter}";
        
        Image img = imageObj.GetComponent<Image>();
        if (!img)
        {
            img = imageObj.AddComponent<Image>();
        }
        img.sprite = imageData.sprite;
        img.preserveAspect = true;
        img.raycastTarget = false;
        
        RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
        if (!rectTransform)
        {
            rectTransform = imageObj.AddComponent<RectTransform>();
        }
        
        // Configurar el RectTransform
        rectTransform.anchorMin = new Vector2(0, 0.5f);
        rectTransform.anchorMax = new Vector2(0, 0.5f);
        rectTransform.pivot = new Vector2(0, 0.5f);
        
        // Guardar información para actualizar posición
        ImagePositionData posData = imageObj.GetComponent<ImagePositionData>();
        if (!posData)
        {
            posData = imageObj.AddComponent<ImagePositionData>();
        }
        posData.charIndex = effect.startIndex;
        posData.sizeMultiplier = imageData.sizeMultiplier;
        
        activeImages.Add(imageObj);
        
        // La imagen estará oculta inicialmente hasta que el typewriter la alcance
        imageObj.SetActive(false);
    }
    
    private void UpdateAllImagePositions()
    {
        foreach (GameObject imageObj in activeImages)
        {
            if (imageObj)
            {
                ImagePositionData posData = imageObj.GetComponent<ImagePositionData>();
                if (posData)
                {
                    UpdateImagePosition(imageObj, posData.charIndex, posData.sizeMultiplier);
                }
            }
        }
    }
    
    private void UpdateImagePosition(GameObject imageObj, int charIndex, float sizeMultiplier)
    {
        if (!textLabel) return;
        
        textLabel.ForceMeshUpdate();
        TMP_TextInfo textInfo = textLabel.textInfo;
        
        // Verificar si el carácter ya es visible (typewriter ha llegado)
        if (charIndex >= textLabel.maxVisibleCharacters)
        {
            imageObj.SetActive(false);
            return;
        }
        
        if (charIndex >= textInfo.characterCount)
        {
            imageObj.SetActive(false);
            return;
        }
        
        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
        
        // Mostrar la imagen solo si el carácter es visible
        imageObj.SetActive(true);
        
        RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
        
        // Obtener posición del carácter en espacio local del texto
        Vector3 bottomLeft = charInfo.bottomLeft;
        Vector3 topRight = charInfo.topRight;
        
        float charHeight = topRight.y - bottomLeft.y;
        float imageSize = charHeight * sizeMultiplier;
        
        Vector2 localPosition = new Vector2(bottomLeft.x, charInfo.baseLine + charInfo.pointSize/2.0f);
        
        rectTransform.anchoredPosition = localPosition;
        rectTransform.sizeDelta = new Vector2(imageSize, imageSize);
    }
    
    private InlineImageData GetImageDataByKey(string key)
    {
        foreach (InlineImageData data in imageDatabase)
        {
            if (data.key == key)
                return data;
        }
        return null;
    }
    
    public void ClearImages()
    {
        foreach (GameObject img in activeImages)
        {
            if (img)
                Destroy(img);
        }
        activeImages.Clear();
        
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }
    
    private void OnDestroy()
    {
        ClearImages();
    }
}

// Componente auxiliar para guardar datos de posición
public class ImagePositionData : MonoBehaviour
{
    public int charIndex;
    public float sizeMultiplier;
}