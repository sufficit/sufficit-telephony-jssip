using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class MediaOptions
    {
        public MediaOptions() { }
        public MediaOptions(bool options) { if(options) DeviceID = "default"; }

        [JsonPropertyName("deviceId")]
        [DataMember(EmitDefaultValue = false)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DeviceID { get; set; }

        [JsonPropertyName("exact")]
        [DataMember(EmitDefaultValue = false)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MediaOptions? Exact { get; set; }

        public static implicit operator MediaOptions(bool options)
        {
            return new MediaOptions(options);
        }
    }
}
