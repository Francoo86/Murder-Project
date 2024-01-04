using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//SI VAN A USAR ARCHIVOS MP3 CONVIERTANLOS A UNA FRECUENCIA DE MUESTREO DE 44100khz.
//EN UNITY CAMBIEN SUS PROPIEDADES A STREAMING Y BAJENLO A 70 EN CALIDAD.

/// <summary>
/// The AudioTrack object that is handled by a channel. Mostly used for environment music.
/// Plays and Stops AudioSource objects.
/// </summary>
public class AudioTrack
{
    private const string TRACK_NAME_FORMAT = "Track - [{0}]";
    public string Name { get; private set; }
    public string path { get; private set; }
    private AudioChannel channel;
    private AudioSource source;
    public GameObject Root => source.gameObject;
    public bool Loop => source.loop;
    public float VolumeCap { get; private set; }
    //public float pitch { get { return source.pitch; } private set { source.pitch = value; } }
    public bool IsPlaying => source.isPlaying;
    public float Volume { get { return source.volume; } set { source.volume = value; } }

    /// <summary>
    /// Creates an AudioTrack object by also creating an AudioSource object.
    /// </summary>
    /// <param name="clip">The AudioClip to use the name.</param>
    /// <param name="loop">Marks the track as loopable.</param>
    /// <param name="startingVol">The starting volume of the track.</param>
    /// <param name="volumeCap">The max volume of the track.</param>
    /// <param name="channel">The channel to be associated with the track.</param>
    /// <param name="mixer">The mixer to be associated (Unity GameObject) usually is the AudioController music mixer.</param>
    /// <param name="filePath">The path of the track.</param>
    public AudioTrack(AudioClip clip, bool loop, float startingVol, float volumeCap, AudioChannel channel, AudioMixerGroup mixer, string filePath)
    {
        Name = clip.name;
        path = filePath;
        this.channel = channel;
        VolumeCap = volumeCap;

        source = CreateSource();
        source.clip = clip;
        source.loop = loop;
        source.volume = startingVol;

        source.outputAudioMixerGroup = mixer;
    }

    /// <summary>
    /// Creates the audio source and links it with the TrackContainer GameObject of the track channel.
    /// </summary>
    /// <returns>The AudioSource element added to the container.</returns>
    private AudioSource CreateSource()
    {
        GameObject trackObj = new GameObject(string.Format(TRACK_NAME_FORMAT, Name));
        trackObj.transform.SetParent(channel.TrackContainer);

        AudioSource source = trackObj.AddComponent<AudioSource>();
        return source;
    }

    /// <summary>
    /// Plays the AudioSource object related to the track.
    /// </summary>
    public void Play()
    {
        source.Play();
    }

    /// <summary>
    /// Stops the AudioSource object related to the track.
    /// </summary>
    public void Stop()
    {
        source.Stop();
    }
}
