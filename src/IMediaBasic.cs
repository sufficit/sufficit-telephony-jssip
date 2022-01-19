using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public interface IMediaBasic
    {
        [JsonPropertyName("video")]
        bool Video { get; }


        [JsonPropertyName("audio")]
        bool Audio { get; }
    }
}
