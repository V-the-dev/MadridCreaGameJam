using TMPro;
using UnityEngine;

public class SimpleTextColorChange : MonoBehaviour
{
    private Color defaultColor = Color.black;
    [SerializeField] private Color hoverColor = Color.orange;

    public TextMeshProUGUI textMeshPro;

    void Start()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.color = defaultColor;
    }

    public void SetHoverColor()
    {
        textMeshPro.color = hoverColor;
    }

    public void SetDefaultColor()
    {
        textMeshPro.color = defaultColor;
    }
}
