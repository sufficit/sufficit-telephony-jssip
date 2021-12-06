using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public enum JsSIPMediaDeviceKind
    {
        UNKNOWN,

        [JsonPropertyName("videoinput")]
        VIDEOINPUT,

        [JsonPropertyName("audioinput")]
        AUDIOINPUT,

        [JsonPropertyName("audiooutput")]
        AUDIOOUTPUT
    }
}
