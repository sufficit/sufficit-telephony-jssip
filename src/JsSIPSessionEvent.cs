using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPSessionEvent
    {
        /// <summary>
        /// Indicates the channel that originate the referenced event
        /// </summary>
        [JsonPropertyName("originator")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JsSIPSessionEventOriginator Originator { get; set; }

        /// <summary>
        /// Hangup cause
        /// </summary>
        [JsonPropertyName("cause")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        public string? Cause { get; set; }
    }
}
