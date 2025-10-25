using UnityEngine;

public class SimpleButtonUI : MonoBehaviour
{

    public void PlayClickSound()
    {
        SoundManager.PlaySound(SoundType.UICLICKBUTTON);
    }
    
    public void PlayHoverSound()
    {
        SoundManager.PlaySound(SoundType.UIHOVERBUTTON);
    }
}
