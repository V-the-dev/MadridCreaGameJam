using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public enum SoundType
{
    BELL1, BELL2, BELL3, BELL4,
    DOGBARK,
    DOGGROWL,
    FENCEOPEN,
    FOOTSTEPS,
    KNOCKLOUD,
    KOCKNORMAL,
    MONEYEARN,
    MONEYPAY,
    STREETAMBIENT,
    MUSIC1,
    MUSIC2,
    UICLICKBUTTON,
    UIHOVERBUTTON
}

[ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    public Dictionary<AudioSourceName, AudioSource> audioSources = new Dictionary<AudioSourceName, AudioSource>();

    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup sfxMixer;
    [SerializeField] private AudioMixerGroup ambientMixer;

    private AudioSource musicSource;
    private AudioSource camSource;

    private bool filtersActive = false;
    private Coroutine fadeRoutine;
    [SerializeField] private float fadeTime=3f;

    private bool hasPlayedMusic = false;
    private bool isInitialScene = true;
    public static SoundManager instance { get; private set; }

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
            if(Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(Application.isPlaying)
                Destroy(gameObject);
            return;
        }

        if (Camera.main != null)
            camSource = Camera.main.GetComponent<AudioSource>();

        if(Application.isPlaying)
            SceneManager.sceneLoaded += OnSceneLoaded;

        RebuildAudioSourcesFromList();
    }

    private void OnDestroy()
    {
        if(Application.isPlaying)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Start()
    {
        if (!Application.isPlaying) return;

        musicSource = SoundManager.instance.getSource(AudioSourceName.MusicSource);

        if(!isInitialScene)
        {
            StartCoroutine(PlayMusic());
            hasPlayedMusic=true;
        }
    }

    
    private IEnumerator PlayMusic()
    {
        yield return null;

        SoundManager.PlaySound(
            SoundType.MUSIC1,
            source: AudioSourceName.MusicSource,
            volume: 1f,
            loop: false
        );

        yield return new WaitUntil(() => musicSource != null && musicSource.clip != null && musicSource.clip.loadState == AudioDataLoadState.Loaded);

        // Si por alguna razón no está sonando, asegura arrancarlo
        if (!musicSource.isPlaying)
            musicSource.Play();

        // Espera a que termine MUSIC1
        yield return new WaitUntil(() => musicSource.time >= musicSource.clip.length - 0.25);

        // Reproduce MUSIC2 en bucle
        SoundManager.PlaySound(
            SoundType.MUSIC2,
            volume: 1f,
            loop: true
        );

        // Desactiva este componente para que no vuelva a ejecutarse
        enabled = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {


        // Rebuild audio sources to account for new scene objects
        RebuildAudioSourcesFromList();

        // Play ambient sound in the new scene
        if(!isInitialScene)
        {
            if (Camera.main != null)
            {
                transform.SetParent(Camera.main.transform);
                transform.localPosition = Vector3.zero; // Align with camera's position
                camSource = Camera.main.GetComponent<AudioSource>();
            }

            SoundManager.PlaySound(
            SoundType.STREETAMBIENT,
            source: AudioSourceName.AmbientSource,
            volume: 0.5f,
            loop: true
            );
        }

        isInitialScene = false;
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

        //Selecciona el AudioSource
        AudioSource targetSource = null;
        if (instance.audioSources != null)
            instance.audioSources.TryGetValue(source, out targetSource);

        //Si no se encontró, intenta reconstruir el diccionario
        if (targetSource == null)
        {
            instance.RebuildAudioSourcesFromList();
            if (instance.audioSources != null)
                instance.audioSources.TryGetValue(source, out targetSource);
        }

        //Si sigue sin encontrarse, usa la cámara
        if (targetSource == null)
            targetSource = instance.camSource;

        //Asigna el mixer correcto
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

        //Configura pitch y looping
        targetSource.pitch = useRandomPitch
            ? UnityEngine.Random.Range(minPitch, maxPitch)
            : 1f;

        targetSource.loop = loop;

        if (loop)
        {
            targetSource.clip = randomClip;
            targetSource.volume = volume;
            targetSource.Play();
        }
        else
        {
            targetSource.clip = randomClip;
            targetSource.volume = volume;
            targetSource.Play();
        }
    }


    public static void ToggleFilters()
    {
        if (instance == null) return;

        if (instance.fadeRoutine != null)
            instance.StopCoroutine(instance.fadeRoutine);

        if (instance.filtersActive)
            instance.fadeRoutine = instance.StartCoroutine(instance.FadeFilters(false, instance.fadeTime));
        else
            instance.fadeRoutine = instance.StartCoroutine(instance.FadeFilters(true, instance.fadeTime));

        instance.filtersActive = !instance.filtersActive;
    }



    private IEnumerator FadeFilters(bool activate, float duration)
    {
        // Define los valores objetivo
        float targetHigh = activate ? 1100f : 10f;
        float targetLow = activate ? 600f : 22000f;
        float targetVol = activate ? 10f : 0f;

        // Lee los valores actuales
        musicMixer.audioMixer.GetFloat("Highpass", out float currentHigh);
        musicMixer.audioMixer.GetFloat("Lowpass", out float currentLow);
        musicMixer.audioMixer.GetFloat("Volume_Music", out float currentVol);

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration); // ← curva suave

            float newHigh = Mathf.Lerp(currentHigh, targetHigh, k);
            float newLow = Mathf.Lerp(currentLow, targetLow, k);
            float newVol = Mathf.Lerp(currentVol, targetVol, k);

            musicMixer.audioMixer.SetFloat("Highpass", newHigh);
            musicMixer.audioMixer.SetFloat("Lowpass", newLow);
            musicMixer.audioMixer.SetFloat("Volume_Music", newVol);

            yield return null;
        }

        // Garantiza el valor final exacto
        musicMixer.audioMixer.SetFloat("Highpass", targetHigh);
        musicMixer.audioMixer.SetFloat("Lowpass", targetLow);
        musicMixer.audioMixer.SetFloat("Volume_Music", targetVol);
    }

    public AudioSource getSource(AudioSourceName name)
    {
        AudioSource source = null;
        instance.audioSources.TryGetValue(name, out source);
        return source;
    }

    private void RebuildAudioSourcesFromList()
    {
        if (audioSources == null)
            audioSources = new Dictionary<AudioSourceName, AudioSource>();

        audioSources.Clear();

        var allSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.InstanceID);

        foreach (var src in allSources)
        {
            if (src == null) continue;

            string cleanName = src.gameObject.name
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("(", "")
                .Replace(")", "")
                .Replace(".", "_");

            if (Enum.TryParse<AudioSourceName>(cleanName, out var enumVal))
            {
                if (!audioSources.ContainsKey(enumVal))
                    audioSources[enumVal] = src;
            }
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

        instance = this;
    }

    //Busca audioSources en la escena, crea un valor de enum de nombre para cada uno y llena el diccionario con ellos
    [MenuItem("Tools/Generate Audio Cosas")]
    private static void GenerateAudioSourceEnum()
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
        foreach (var src in FindObjectsByType<AudioSource>(FindObjectsSortMode.InstanceID))
            sources.Add(src);

        sources.Sort((a, b) => string.Compare(a.gameObject.name, b.gameObject.name, StringComparison.Ordinal));

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

            if (Enum.TryParse<AudioSourceName>(cleanName, out AudioSourceName enumValue))
                instance.audioSources[enumValue] = src;
        }

        // Genera el enum
        string enumCode = "public enum AudioSourceName\n{\n";
        foreach (string name in enumNames)
            enumCode += $"    {name},\n";
        enumCode += "}\n";

        string path = "Assets/Game/Scripts/Enums/AudioSourceName.cs";
        System.IO.File.WriteAllText(path, enumCode);
        AssetDatabase.Refresh();

        // Marca cambios como sucios para guardar automáticamente
        EditorUtility.SetDirty(instance);
        if (instance.gameObject.scene.IsValid())
            EditorSceneManager.MarkSceneDirty(instance.gameObject.scene);

        Debug.Log($"{instance.audioSources.Count} AudioSources detectados.\nEnum generado en: {path}");
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
