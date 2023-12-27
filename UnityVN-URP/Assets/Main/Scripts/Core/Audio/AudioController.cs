using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public const string MUSIC_VOLUME_PARAMETER_NAME = "MusicVolume";
    public const string SFX_VOLUME_PARAMETER_NAME = "SFXVolume";
    public const float MUTED_VOLUME_LEVEL = -80f;

    private const string SFX_PARENT_NAME = "SFX";

    public const float TRACK_TRANSITION_SPEED = 1f;

    private const string SFX_NAME_FORMAT = "SFX - [{0}]";
    //Como es un controlador entonces usamos singleton.
    public static AudioController Instance {  get; private set; }
    public Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>();

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;

    public AnimationCurve audioFalloffCurve;

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
            Debug.LogWarning($"Couldn't find the {filePath} audio asset. Please ensure that it is on the Resources folder.");
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

    //TODO: Refactor onto one method.
    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true, float startVol = 0f, float capVol = 1f)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null)
        {
            Debug.LogWarning($"Couldn't find the {filePath} audio asset. Please ensure that it is on the Resources folder.");
            return null;
        }

        return PlayTrack(clip, channel, loop, startVol, capVol, filePath);
    }

    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startVol = 0f, float capVol = 1f, string filePath = "")
    {
        AudioChannel audioChannel = TryToGetChannel(channel, forceCreation: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startVol, capVol, filePath);
        return track;
    }

    public void StopTrack(int channel)
    {
        AudioChannel chan = TryToGetChannel(channel, forceCreation: false);

        if (chan == null)
            return;

        chan.StopTrack();
    }

    public void StopTrack(string trackName)
    {
        trackName = trackName.ToLower();
        foreach(var channel in channels.Values)
        {
            AudioTrack chanTrack = channel.CurrentTrack;
            if(chanTrack != null && chanTrack.Name.ToLower() == trackName)
            {
                chanTrack.Stop();
                return;
            }
        }
    }

    public AudioChannel TryToGetChannel(int channelNum, bool forceCreation = false)
    {
        AudioChannel channel = null;

        if(channels.TryGetValue(channelNum, out channel))
        {
            return channel;
        }
        else if(forceCreation)
        {
            channel = new AudioChannel(channelNum);
            channels.Add(channelNum, channel);
            return channel;
        }

        return null;
    }

    public void SetMusicVolume(float volume, bool muted)
    {
        volume = muted ? MUTED_VOLUME_LEVEL : volume;//audioFalloffCurve.Evaluate(volume);
        Debug.Log($"Evaluating this thing: {volume}");
        musicMixer.audioMixer.SetFloat(MUSIC_VOLUME_PARAMETER_NAME, volume);
    }
    public void SetSFXVolume(float volume, bool muted)
    {
        volume = muted ? MUTED_VOLUME_LEVEL : volume;
        sfxMixer.audioMixer.SetFloat(SFX_VOLUME_PARAMETER_NAME, volume);
    }
}
