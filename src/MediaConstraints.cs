using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class MediaConstraints
    {
        //{ video: { deviceId: { exact: cameraDevice.deviceId } } }
        [JsonPropertyName("video")]
        public MediaOptions Video { get; set; }


        [JsonPropertyName("audio")]
        public MediaOptions Audio { get; set; }
    }
}
