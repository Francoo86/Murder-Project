using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//SI VAN A USAR ARCHIVOS MP3 CONVIERTANLOS A UNA FRECUENCIA DE MUESTREO DE 44100khz.
//EN UNITY CAMBIEN SUS PROPIEDADES A STREAMING Y BAJENLO A 70 EN CALIDAD.
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

    private AudioSource CreateSource()
    {
        GameObject trackObj = new GameObject(string.Format(TRACK_NAME_FORMAT, Name));
        trackObj.transform.SetParent(channel.TrackContainer);

        AudioSource source = trackObj.AddComponent<AudioSource>();
        return source;
    }

    public void Play()
    {
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}
