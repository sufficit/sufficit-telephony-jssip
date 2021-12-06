using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class MediaDeviceGroup
    {
        public MediaDeviceGroup()
        {
            AudioInput = AudioOuput = VideoInput = string.Empty;
        }

        public void Update(JsSIPMediaDeviceKind kind, string id)
        {
            switch (kind)
            {
                case JsSIPMediaDeviceKind.AUDIOINPUT: {
                        if (AudioInput != id)
                        {
                            AudioInput = id;
                            Notify();
                        }
                        break; 
                    }
                case JsSIPMediaDeviceKind.VIDEOINPUT:
                    {
                        if (VideoInput != id)
                        {
                            VideoInput = id;
                            Notify();
                        }
                        break;
                    }
                case JsSIPMediaDeviceKind.AUDIOOUTPUT:
                    {
                        if (AudioOuput != id)
                        {
                            AudioOuput = id;
                            Notify();
                        }
                        break;
                    }
                default: break;
            }
        }

        protected virtual void Notify()
        {
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event handler for device changed
        /// </summary>
        public event EventHandler? OnChanged;

        /// <summary>
        ///  Microphone Device ID
        /// </summary>
        public string AudioInput { get; set; }

        /// <summary>
        /// Sound Device ID
        /// </summary>
        public string AudioOuput { get; set; }

        /// <summary>
        /// Camera Device ID
        /// </summary>
        public string VideoInput { get; set; }
    }
}
