using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    private const string SFX_PARENT_NAME = "SFX";
    private const string MUSIC_PARENT_NAME = "Music";

    private const string SFX_NAME_FORMAT = "SFX - [{0}]";
    //Como es un controlador entonces usamos singleton.
    public static AudioController Instance {  get; private set; }

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;

    //Usar al padre de SFX.
    private Transform sfxRoot;


    private void Awake()
    {
        if (Instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else {
            DestroyImmediate(gameObject);
            return;
        }

        sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
        sfxRoot.SetParent(transform);
    }

    //TODO: Add KeyValue JSON thing to make it somewhat similar to get files with only names.
    public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixGroup = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null)
        {
            Debug.LogWarning($"Couldn't find the {filePath} audio asset.");
            return null;
        }

        return PlaySoundEffect(clip, mixGroup, volume, pitch, loop);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixGroup = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        string formattedName = string.Format(SFX_NAME_FORMAT, clip.name);
        AudioSource effectSrc = new GameObject(formattedName).AddComponent<AudioSource>();
        effectSrc.transform.SetParent(sfxRoot);
        effectSrc.transform.position = sfxRoot.position;
        effectSrc.clip = clip;

        if (mixGroup == null)
            mixGroup = sfxMixer;

        effectSrc.outputAudioMixerGroup = mixGroup;
        effectSrc.volume = volume;
        //No es un sonido en 3D.
        effectSrc.spatialBlend = 0;
        effectSrc.pitch = pitch;
        effectSrc.loop = loop;

        effectSrc.Play();

        if (!loop)
            Destroy(effectSrc.gameObject, (clip.length / pitch) + 1);

        return effectSrc;
    }

    public void StopSoundEffects(string soundName)
    {
        soundName =  soundName.ToLower();
        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();

        foreach (AudioSource source in sources)
        {
           if(source.clip.name.ToLower() == soundName)
            {
                Destroy(source.gameObject);
                return;
            }
        }
    }

    public void StopSoundEffects(AudioClip clip) => StopSoundEffects(clip.name);
}
