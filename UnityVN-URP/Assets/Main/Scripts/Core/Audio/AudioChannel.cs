using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Esto es parte del tutorial de audio.

/// <summary>
/// Class that makes possible making multiple tracks to be played at the same time.
/// </summary>
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

    /// <summary>
    /// Creates the AudioChannel object and creates a GameObject to be associated to the controller as a child.
    /// </summary>
    /// <param name="channel">The channel num.</param>
    public AudioChannel(int channel) {
        ChannelIndex = channel;

        TrackContainer =  new GameObject(string.Format(TRACK_CONTAINER_NAME_FORMAT, channel)).transform;
        TrackContainer.transform.SetParent(AudioController.Instance.transform);
    }
    
    /// <summary>
    /// Plays a track by AudioClip data, if it doesn't exist then creates a new one and plays it on the new one.
    /// </summary>
    /// <param name="clip">AudioClip data to get the name.</param>
    /// <param name="loop">Marks the track as loopable.</param>
    /// <param name="startVol">The starting volume of the track.</param>
    /// <param name="volCap">The max volume that the track can have.</param>
    /// <param name="filePath">The filePath of the track.</param>
    /// <returns>The track that will be played.</returns>
    public AudioTrack PlayTrack(AudioClip clip, bool loop, float startVol, float volCap, string filePath)
    {
        if(TryToGetTrack(clip.name, out AudioTrack existingTrack))
        {
            if (!existingTrack.IsPlaying)
                existingTrack.Play();

            SetActiveTrack(existingTrack);

            return existingTrack;
        }

        AudioTrack track = new AudioTrack(clip, loop, startVol, volCap, this, AudioController.Instance.musicMixer, filePath);
        track.Play();

        //Save this track to current one.
        SetActiveTrack(track);

        return track;
    }

    /// <summary>
    /// Sets the channel active track and volumes it.
    /// </summary>
    /// <param name="track">The track.</param>
    private void SetActiveTrack(AudioTrack track)
    {
        if (!tracks.Contains(track))
            tracks.Add(track);

        CurrentTrack = track;

        TryStartVolumeLeveling();
    }

    /// <summary>
    /// Tries to get a track by name.
    /// </summary>
    /// <param name="name">The track name.</param>
    /// <param name="value">The value that gets the track.</param>
    /// <returns>Was retrieved succesfully.</returns>
    public bool TryToGetTrack(string name, out AudioTrack value)
    {
        name = name.ToLower();

        foreach(var track in tracks)
        {
            if (track == null)
                continue;

            if(track.Name.ToLower() == name)
            {
                value = track;
                return true;
            }
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Tries to start the volume leveling of the current track.
    /// </summary>
    private void TryStartVolumeLeveling()
    {
        if (!IsLevelingVolume)
        {
            co_VolumeLeveling = AudioController.Instance.StartCoroutine((VolumeLeveling()));
        }
    }

    /// <summary>
    /// Makes the current track to go to 0 volume and then destroys it.
    /// </summary>
    /// <returns>The IEnumerator to be yielded with the AudioController.</returns>
    private IEnumerator VolumeLeveling()
    {
        //Deberiamos tener al menos un track dentro de la lista.
        while(CurrentTrack != null && (tracks.Count > 1 || CurrentTrack.Volume != CurrentTrack.VolumeCap) || (CurrentTrack == null && tracks.Count > 0))
        {
            //Contar en reversa para poder realizar el efecto.
            for(int i = tracks.Count - 1; i >= 0; i--)
            {
                AudioTrack track = tracks[i];
                if (track == null) continue;

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

    /// <summary>
    /// Removes a track from the list and destroys it.
    /// </summary>
    /// <param name="track">The track to be removed</param>
    private void DestroyTrack(AudioTrack track)
    {
        if(tracks.Contains(track))
        {
            tracks.Remove(track);
        }

        Object.Destroy(track.Root);
    }

    /// <summary>
    /// Stops the track and destroys it.
    /// </summary>
    /// <param name="inmediate">Marks this as instant stop.</param>
    public void StopTrack(bool inmediate = false)
    {
        if(CurrentTrack == null) { return; }
        if (inmediate)
        {
            DestroyTrack(CurrentTrack);
            SetActiveTrack(null);
        }
        else 
        {
            CurrentTrack = null;
            TryStartVolumeLeveling();
        }
    }
}
