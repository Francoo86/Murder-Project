using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Controls the audio logic of the game.
/// </summary>
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

    //public AnimationCurve audioFalloffCurve;

    //Usar al padre de SFX.
    private Transform sfxRoot;


    /// <summary>
    /// Setups the controller, and resets the instance if the scene was changed.
    /// </summary>
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
    /// <summary>
    /// Plays sound effect by passing the filepath.
    /// </summary>
    /// <param name="filePath">The audio path.</param>
    /// <param name="volume">The volume of the audio in a scale of 0-1.</param>
    /// <param name="pitch">How pitched is the audio.</param>
    /// <param name="loop">Marks the sound as loopable.</param>
    /// <returns>The AudioSource object to be played on the scene.</returns>
    public AudioSource PlaySoundEffect(string filePath, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if(clip == null)
        {
            Debug.LogWarning($"Couldn't find the {filePath} audio asset. Please ensure that it is on the Resources folder.");
            return null;
        }

        return PlaySoundEffect(clip, volume, pitch, loop);
    }

    /// <summary>
    /// Plays a sound by passing a loaded AudioClip object.
    /// </summary>
    /// <param name="clip">The loaded audio object.</param>
    /// <param name="volume">The volume of audio (scales 0-1).</param>
    /// <param name="pitch">The pitch of the sound.</param>
    /// <param name="loop">Marks the sound as loopable.</param>
    /// <returns>The AudioSource object to be played on the scene.</returns>
    public AudioSource PlaySoundEffect(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
    {
        string formattedName = string.Format(SFX_NAME_FORMAT, clip.name);
        AudioSource effectSrc = new GameObject(formattedName).AddComponent<AudioSource>();
        effectSrc.transform.SetParent(sfxRoot);
        effectSrc.transform.position = sfxRoot.position;
        effectSrc.clip = clip;

        /*
        if (mixGroup == null)
            mixGroup = sfxMixer;*/

        effectSrc.outputAudioMixerGroup = sfxMixer;
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

    /// <summary>
    /// Stop the sound by name and destroys the GameObject of the scene.
    /// </summary>
    /// <param name="soundName">The sound name.</param>
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

    /// <summary>
    /// Stops the sound by the AudioClip data object.
    /// </summary>
    /// <param name="clip">The AudioClip object.</param>
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

    /// <summary>
    /// Plays a track by the AudioClip data.
    /// </summary>
    /// <param name="clip">The AudioClip data.</param>
    /// <param name="channel">The channel where the track should play.</param>
    /// <param name="loop">Marks the track loopable.</param>
    /// <param name="startVol">The volume that the track will start.</param>
    /// <param name="capVol">The max volume that the track can be have.</param>
    /// <param name="filePath">The path of the track.</param>
    /// <returns>The AudioTrack that will be played.</returns>
    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startVol = 0f, float capVol = 1f, string filePath = "")
    {
        AudioChannel audioChannel = TryToGetChannel(channel, forceCreation: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startVol, capVol, filePath);
        return track;
    }

    /// <summary>
    /// Stops the current track of the requested channel.
    /// </summary>
    /// <param name="channel">The channel num.</param>
    public void StopTrack(int channel)
    {
        AudioChannel chan = TryToGetChannel(channel, forceCreation: false);

        if (chan == null)
            return;

        chan.StopTrack();
    }

    /// <summary>
    /// Stops the track by name associated to a channel.
    /// </summary>
    /// <param name="trackName">The track name.</param>
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

    /// <summary>
    /// Tries to get an AudioChannel by number.
    /// </summary>
    /// <param name="channelNum">The channel num.</param>
    /// <param name="forceCreation">The channel should be created if doesn't exists.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Sets the music volume of the environment.
    /// </summary>
    /// <param name="volume">The volume in dB (being -80 muted and 20 the greatest).</param>
    /// <param name="muted">Mute instantly.</param>
    public void SetMusicVolume(float volume, bool muted)
    {
        volume = muted ? MUTED_VOLUME_LEVEL : volume;//audioFalloffCurve.Evaluate(volume);
        Debug.Log($"Evaluating this thing: {volume}");
        musicMixer.audioMixer.SetFloat(MUSIC_VOLUME_PARAMETER_NAME, volume);
    }

    /// <summary>
    /// Sets the SFX volume when playing a sound.
    /// </summary>
    /// <param name="volume">The volume in dB (being -80 muted and 20 the greatest).</param>
    /// <param name="muted">Mute instantly.</param>
    public void SetSFXVolume(float volume, bool muted)
    {
        volume = muted ? MUTED_VOLUME_LEVEL : volume;
        sfxMixer.audioMixer.SetFloat(SFX_VOLUME_PARAMETER_NAME, volume);
    }
}
