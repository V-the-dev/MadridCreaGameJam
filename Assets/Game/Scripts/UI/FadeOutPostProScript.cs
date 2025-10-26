using UnityEngine;

public class FadeOutPostProScript : MonoBehaviour
{
    //Esta funcion se llama desde el animator cuando termina la animación
    public void FinishFadeOut()
    {
        GameManager.Instance.FinishedFadeOut();
    }
}
