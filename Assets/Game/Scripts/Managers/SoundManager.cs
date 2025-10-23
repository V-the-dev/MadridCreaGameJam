using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

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
    MONEYPAY,
    STREETAMBIENT
}

[ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    [SerializeField] private AudioSource[] audioSources;

    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup sfxMixer;
    [SerializeField] private AudioMixerGroup ambientMixer;

    AudioSource camSource;

    public static SoundManager instance { get; private set; }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            camSource = Camera.main.GetComponent<AudioSource>();
        }


    }

    private void Start()
    {
        SoundManager.PlaySound(SoundType.STREETAMBIENT, null, volume: 0.5f, loop: true);
    }


    public static void PlaySound(
        SoundType sound,
        AudioSource source = null,
        float volume = 1f,
        bool useRandomPitch = false,
        bool loop = false,
        float minPitch = 0.95f,
        float maxPitch = 1.05f)
    {
        if (instance == null)
        {
            Debug.LogWarning("SoundManager instance not found!");
            return;
        }

        var soundData = instance.soundList[(int)sound];
        AudioClip[] clips = soundData.Sounds;
        if (clips == null || clips.Length == 0) return;

        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        // Selecciona el AudioSource
        AudioSource targetSource = source ? source : instance.camSource;

        // Asigna el mixer correcto según categoría
        switch (soundData.category)
        {
            case SoundCategory.Music:
                targetSource.outputAudioMixerGroup = instance.musicMixer;
                break;
            case SoundCategory.SFX:
                targetSource.outputAudioMixerGroup = instance.sfxMixer;
                break;
            case SoundCategory.Ambient:
                targetSource.outputAudioMixerGroup = instance.ambientMixer;
                break;
        }

        // Configura el pitch si está activado
        targetSource.pitch = useRandomPitch
            ? UnityEngine.Random.Range(minPitch, maxPitch)
            : 1f;

        // Configura looping, se para con source.Stop();
        targetSource.loop = loop;

        if (loop)
        {
            targetSource.clip = randomClip;
            targetSource.Play();
        }
        else
        {
            targetSource.PlayOneShot(randomClip, volume);
        }
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

        instance=this;
    }

    [MenuItem("Tools/List All AudioSources")]
    private static void ListAllAudioSources()
    {
        if (instance == null)
        {
            instance = FindAnyObjectByType<SoundManager>();
            if (instance == null)
            {
                var go = new GameObject("SoundManager");
                instance = go.AddComponent<SoundManager>();
            }
        }

        var sources = new List<AudioSource>();

        foreach (var src in FindObjectsByType<AudioSource>(sortMode: FindObjectsSortMode.InstanceID))
            sources.Add(src);

        sources.Sort((a, b) => string.Compare(a.gameObject.name, b.gameObject.name, StringComparison.Ordinal));
        instance.audioSources = sources.ToArray();

        Debug.Log($"Total audio sources: {sources.Count}");
    }
#endif
}

public enum SoundCategory
{
    SFX,
    Music,
    Ambient
}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    public SoundCategory category;
    [SerializeField] private AudioClip[] sounds;
    public AudioClip[] Sounds => sounds;
}
