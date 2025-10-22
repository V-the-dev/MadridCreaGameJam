using UnityEngine;
using System;

public enum SoundType
{
    BELL,
    DOGBARK,
    DOGGROWL,
    FENCEOPEN,
    FOOTSTEPS,
    KNOCKLOUD,
    KOCKNORMAL,
    MONEYEARN,
    MONEYPAY
}

[RequireComponent(typeof(AudioSource)),ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;

    AudioSource camSource;

    public static SoundManager instance { get; private set; } = null;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            camSource = gameObject.GetComponent<AudioSource>();
        }
    }


    public static void PlaySound(SoundType sound, AudioSource source=null, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if(source)
            source.PlayOneShot(randomClip, volume);
        else
            instance.camSource.PlayOneShot(randomClip, volume);
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get=> sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}
