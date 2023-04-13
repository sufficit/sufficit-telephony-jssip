using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        [JsonConverter(typeof(EnumConverter<JsSIPSessionCause>))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        public JsSIPSessionCause? Cause { get; set; }

        /// <summary>
        /// Extra message information
        /// </summary>
        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public JsonElement Message { get; set; }
    }
}
