using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class MediaOptions
    {
        [JsonPropertyName("deviceId")]
        public string DeviceID { get; set; }

        public bool Exact { get; set; }
    }
}
