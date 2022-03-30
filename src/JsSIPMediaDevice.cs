using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPMediaDevice
    {
        public JsSIPMediaDevice()
        {
            this.ID = string.Empty;
            this.Label = string.Empty;            
        }

        [JsonPropertyName("deviceId")]
        public string ID { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("kind")]
        public string? Kind { get; set; }

        //[JsonPropertyName("kind")]
        //public JsSIPMediaDeviceKind Kind { get; set; }
    }
}
