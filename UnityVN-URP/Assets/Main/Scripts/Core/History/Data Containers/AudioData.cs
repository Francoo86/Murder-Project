using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace History
{

    public class AudioData
    {
        public int channel = 0;
        public string trackName;
        public string trackPath;
        public float trackVolume;
        //public float trackPitch;
        public bool loop;

        public AudioData(AudioChannel channel)
        {
            this.channel = channel.ChannelIndex;

            if (channel.CurrentTrack == null)
                return;

            var track = channel.CurrentTrack;
            trackName = track.Name;
            trackPath = track.path;
            trackVolume = track.VolumeCap;
            //trackPitch = track.pitch;
            loop = track.Loop;
        }

        public static List<AudioData> Capture() 
        {
            List<AudioData> audioChannels = new List<AudioData>();

            foreach (var channel in AudioController.Instance.channels)
            {
                if (channel.Value.CurrentTrack == null)
                    continue;

                AudioData data = new AudioData(channel.Value);
                audioChannels.Add(data);
            }

            return audioChannels;
        }


        public static void Apply(List<AudioData> data) 
        {
            List<int> cache = new List<int>();

            foreach (var channelData in data)
            {
                AudioChannel channel = AudioController.Instance.TryToGetChannel(channelData.channel, forceCreation: true);
                if (channel.CurrentTrack == null || channel.CurrentTrack.Name != channelData.trackName)
                {
                    AudioClip clip = HistoryCache.LoadAudio(channelData.trackPath);
                    if (clip != null)
                    {
                        channel.StopTrack(inmediate: true);
                        channel.PlayTrack(clip, channelData.loop, channelData.trackVolume, channelData.trackVolume, channelData.trackPath);
                    }
                    else
                        Debug.LogWarning("$History State: Could not load audio track '{channelData.trackPath}'");
                }
                cache.Add(channelData.channel);
            }

            foreach (var channel in AudioController.Instance.channels)
            {
                if (!cache.Contains(channel.Value.ChannelIndex))
                    channel.Value.StopTrack(inmediate: true);
            }
        }
    }
}
