using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Esto es parte del tutorial de audio.
public class AudioChannel : MonoBehaviour
{
    private const string TRACK_CONTAINER_NAME_FORMAT = "Channel - [{0}]";
    public int ChannelIndex { get; private set; }
    //El padre que guardará todas las pistas de audio.
    private List<AudioTrack> tracks = new List<AudioTrack>();
    public Transform TrackContainer { get; private set; } = null;
    public AudioChannel(int channel) {
        ChannelIndex = channel;

        TrackContainer =  new GameObject(string.Format(TRACK_CONTAINER_NAME_FORMAT, channel)).transform;
        TrackContainer.transform.SetParent(AudioController.Instance.transform);
    }
    
    public AudioTrack PlayTrack(AudioClip clip, bool loop, float startVol, float volCap, string filePath = "")
    {
        if(TryToGetTrack(clip.name, out AudioTrack existingTrack))
        {
            if (!existingTrack.IsPlaying)
                existingTrack.Play();

            return existingTrack;
        }

        AudioTrack track = new AudioTrack(clip, loop, startVol, volCap, this, AudioController.Instance.musicMixer);
        track.Play();
        return track;
    }

    public bool TryToGetTrack(string name, out AudioTrack value)
    {
        name = name.ToLower();

        foreach(var track in tracks)
        {
            if(track.Name.ToLower() == name)
            {
                value = track;
                return true;
            }
        }

        value = null;
        return false;
    }
}
