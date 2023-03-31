using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP.SIP
{
    public class RegisterEventResponse
    {
        [JsonPropertyName("headers")]
        public NameValueCollection Headers { get; set; } = default!;
    }
}
