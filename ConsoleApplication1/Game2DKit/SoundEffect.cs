using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game2DKit.SoundMangement
{
    public class SoundEffect
    {
        public string SoundFile { get; set; }

        public SoundEffect()
        {            
        }

        public SoundEffect(string fileDir)
        {
            SoundFile = fileDir;         
        }

        public void Play()
        {
            try
            {                
                var player = new WMPLib.WindowsMediaPlayer();
                player.URL = this.SoundFile;
            }
            catch //pokemon exception handling
            {
                
            }            
        }

    }
}
