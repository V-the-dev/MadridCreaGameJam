using UnityEngine;

public class PlaySoundEnter : StateMachineBehaviour
{
    [SerializeField] private SoundType sound;
    [SerializeField] private AudioSourceName source = AudioSourceName.Main_Camera;
    [SerializeField,Range(0,1)] private float volume = 1f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.PlaySound(sound,source, volume);
    }

}
