using UnityEngine;

public class FadeOutPostProScript : MonoBehaviour
{
    public void FinishFadeOut()
    {
        GameManager.Instance.FinishedFadeOut();
    }
}
