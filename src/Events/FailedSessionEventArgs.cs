using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP.Events
{
    public class FailedSessionEventArgs : JsSIPEvent
    {
        public string Originator { get; set; } = string.Empty;

        /// <summary>
        /// Hangup cause
        /// </summary>
        [JsonPropertyName("cause")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        public JsSIPSessionCause Cause { get; set; }

        public string? ToJsonString()
        {
            try { return JsonSerializer.Serialize(this); } catch { return null; }
        }
    }
}
