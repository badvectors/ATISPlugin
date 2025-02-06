using System;
using vatsys;

namespace ATISPlugin
{
    public class ATISAudio
    {
        public ATISAudio() 
        {
            Id = Guid.NewGuid();
        }

        public ATISAudio(byte[] audio, int atisIndex, string? callsign, uint frequency, Coordinate? location, TimeSpan interval) : this()
        { 
            Audio = audio;
            ATISIndex = atisIndex;
            Callsign = callsign;
            Frequency = frequency;
            VisPoint = location;
            Duration = interval;
        }

        public Guid Id { get; set; }
        public byte[]? Audio { get; set; }
        public int ATISIndex { get; set; }
        public string? Callsign { get; set; }
        public uint Frequency { get; set; }
        public Coordinate? VisPoint { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
