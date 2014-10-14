using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Game2DKit.SoundMangement
{
    public class Song : GenericProcess
    {
        public SoundEffect Track { get; set; }

        //time in seconds
        public int Duration { get; set; }

        public Song(SoundEffect sound)
        {
            this.Track = sound;
            //20 seconds default
            this.Duration = 20;
        }

        public Song(SoundEffect sound, int duration)
        {
            this.Track = sound;
            this.Duration = duration;
        }

        //this method will be one thread's excecution
        //will play the track for a time equals the duration, then start over
        protected override void work()
        {            
            while (running)
            {
                try
                {
                    //this.Track.Play();
                    var player = new WMPLib.WindowsMediaPlayer();
                    player.URL = this.Track.SoundFile;
                    Thread.Sleep(this.Duration * 1000);
                }
                catch //pokemon exception handling
                { 
                
                }
            }            
        }

    }
}
