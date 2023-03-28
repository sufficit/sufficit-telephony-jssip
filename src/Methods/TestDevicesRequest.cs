using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP.Methods
{
    public class TestDevicesRequest
    {
        [JsonPropertyName("audio")]
        public bool Audio { get; set; }

        [JsonPropertyName("video")]
        public bool Video { get; set; }
    }
}
