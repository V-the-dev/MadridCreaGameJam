using UnityEngine;

[ExecuteInEditMode]
public class EditorMusicPlayer : MonoBehaviour
{
    public bool playNow = false;

    private void Update()
    {
        if (!Application.isPlaying && playNow)
        {
            playNow = false;

            SoundManager.PlaySound(
                SoundType.MUSIC1,
                source: AudioSourceName.MusicSource,
                volume: 1f,
                loop: false
            );
        }
    }
}
