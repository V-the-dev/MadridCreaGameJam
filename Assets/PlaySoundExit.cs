using UnityEngine;

public class PlaySoundExit : StateMachineBehaviour
{
    [SerializeField] private SoundType sound;
    [SerializeField] private AudioSource source=null;
    [SerializeField, Range(0, 1)] private float volume = 1f;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SoundManager.PlaySound(sound,source ,volume);
    }

}
