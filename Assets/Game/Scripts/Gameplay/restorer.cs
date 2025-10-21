using UnityEngine;

public class RestoreAlpha : MonoBehaviour
{
    [ContextMenu("Restore All Renderer Alphas")]
    void Restore()
    {
        foreach (var r in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            foreach (var mat in r.sharedMaterials)
            {
                if (mat != null && mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = 1f;
                    mat.color = c;
                }
            }
        }
        Debug.Log("✔ Todos los materiales restaurados a alpha = 1");
    }
}
