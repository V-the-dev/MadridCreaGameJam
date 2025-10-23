using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    BELL1,BELL2,BELL3,BELL4,
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
    public Dictionary<AudioSourceName, AudioSource> audioSources=new Dictionary<AudioSourceName, AudioSource>();

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
        SoundManager.PlaySound(SoundType.STREETAMBIENT, volume: 0.5f, loop: true);
        Invoke("delayedplay", 5f);

    }

    private void delayedplay()
    {
        SoundManager.PlaySound(SoundType.BELL1, AudioSourceName.Campanario, volume: 0.5f, loop: false);

        SoundManager.PlaySound(SoundType.BELL2, AudioSourceName.Campanario, volume: 0.5f, loop: false,useRandomPitch:true,minPitch:0.1f,maxPitch:0.5f);
    }


    public static void PlaySound(
        SoundType sound,
        AudioSourceName source = AudioSourceName.Main_Camera,
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
        AudioSource targetSource;
        if (!instance.audioSources.TryGetValue(source, out targetSource))
            targetSource = instance.camSource;


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

    //Busca audioSources en la escena, crea un valor de enum de nombre para cada uno y llena el diccionario con ellos
    [MenuItem("Tools/Generate Audio Cosas")]
    private static void GenerateAudioSourceEnum()
    {
        //toma instancia o crea una
        if (instance == null)
        {
            instance = FindAnyObjectByType<SoundManager>();
            if (instance == null)
            {
                var go = new GameObject("SoundManager");
                instance = go.AddComponent<SoundManager>();
            }
        }

        //toma todos los audiosources en la escena
        var sources = new List<AudioSource>();
        foreach (var src in FindObjectsByType<AudioSource>(FindObjectsSortMode.InstanceID))
            sources.Add(src);

        sources.Sort((a, b) => string.Compare(a.gameObject.name, b.gameObject.name, StringComparison.Ordinal));

        //limpia y llena el diccionario
        instance.audioSources.Clear();
        List<string> enumNames = new List<string>();

        foreach (var src in sources)
        {
            string cleanName = src.gameObject.name
            .Replace(" ", "_")
            .Replace("-", "_")
            .Replace("(", "")
            .Replace(")", "")
            .Replace(".", "_");

            enumNames.Add(cleanName);

            //permite llenar el diccionario con valores que todavia no existen en el enum
            if (System.Enum.TryParse<AudioSourceName>(cleanName, out AudioSourceName enumValue))
            {
                instance.audioSources[enumValue] = src;
            }
        }

        //genera el enum en el path indicado
        string enumCode = "public enum AudioSourceName\n{\n";
        foreach (string name in enumNames)
            enumCode += $"    {name},\n";
        enumCode += "}\n";

        string path = "Assets/Game/Scripts/Enums/AudioSourceName.cs";
        System.IO.File.WriteAllText(path, enumCode);
        AssetDatabase.Refresh();

        Debug.Log($"AudioSources: {instance.audioSources.Count} \nEnum generado en: Assets/Game/Scripts/Enums/AudioSourceName.cs");
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
