using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracking
{
    public class EngineStatus
    {
        public bool playing = false;
        public bool recording = false;
        
        internal void play()
        {
            this.recording = false;
            this.playing = true;
        }

        internal void record()
        {
            this.playing = false;
            this.recording = true;
        }

        internal void stop()
        {
            this.playing = false;
            this.recording = false;
        }
    }
}
