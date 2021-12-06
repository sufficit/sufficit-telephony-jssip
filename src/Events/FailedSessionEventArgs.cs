using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP.Events
{
    public class FailedSessionEventArgs : JsSIPEvent
    {
        public string Originator { get; set; } = string.Empty;

        public string Cause { get; set; } = string.Empty;

        public string ToJsonString() => JsonSerializer.Serialize(this);
    }
}
