using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Esto es parte del tutorial de audio.
public class AudioChannel
{
    private const string TRACK_CONTAINER_NAME_FORMAT = "Channel - [{0}]";
    public int ChannelIndex { get; private set; }
    public AudioTrack CurrentTrack { get; private set; } = null;
    //El padre que guardará todas las pistas de audio.
    private List<AudioTrack> tracks = new List<AudioTrack>();
    public Transform TrackContainer { get; private set; } = null;
    //Darle un efecto de entrada/salida así como las peliculas.
    public bool IsLevelingVolume => co_VolumeLeveling != null;
    Coroutine co_VolumeLeveling = null;
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

            SetActiveTrack(existingTrack);

            return existingTrack;
        }

        AudioTrack track = new AudioTrack(clip, loop, startVol, volCap, this, AudioController.Instance.musicMixer);
        track.Play();

        //Save this track to current one.
        SetActiveTrack(track);

        return track;
    }

    private void SetActiveTrack(AudioTrack track)
    {
        if (!tracks.Contains(track))
            tracks.Add(track);

        CurrentTrack = track;

        TryStartVolumeLeveling();
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

    private void TryStartVolumeLeveling()
    {
        if (!IsLevelingVolume)
        {
            co_VolumeLeveling = AudioController.Instance.StartCoroutine((VolumeLeveling()));
        }
    }

    private IEnumerator VolumeLeveling()
    {
        //Deberiamos tener al menos un track dentro de la lista.
        while(CurrentTrack != null && (tracks.Count > 1 || CurrentTrack.Volume != CurrentTrack.VolumeCap))
        {
            //Contar en reversa para poder realizar el efecto.
            for(int i = tracks.Count - 1; i >= 0; i--)
            {
                AudioTrack track = tracks[i];
                float targetVolume = CurrentTrack == track ? track.VolumeCap : 0;

                //No queremos afectar a nuestra pista actual, para cuando termine la interpolación.
                if(track == CurrentTrack && track.Volume == targetVolume)
                {
                    continue;
                }

                track.Volume = Mathf.MoveTowards(track.Volume, targetVolume, AudioController.TRACK_TRANSITION_SPEED * Time.deltaTime);

                if(track == CurrentTrack && track.Volume == 0)
                {
                    DestroyTrack(track);
                }
            }
            yield return null;
        }

        co_VolumeLeveling = null;
    }

    private void DestroyTrack(AudioTrack track)
    {
        if(tracks.Contains(track))
        {
            tracks.Remove(track);
        }

        Object.Destroy(track.Root);
    }

    public void StopTrack()
    {
        if(CurrentTrack == null) { return; }

        CurrentTrack = null;
        TryStartVolumeLeveling();
    }
}
