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
    }
}
