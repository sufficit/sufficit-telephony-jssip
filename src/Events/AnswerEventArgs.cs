using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP.Events
{
    public class AnswerEventArgs
    {
        [JsonPropertyName("mediaConstraints")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MediaConstraints? MediaConstraints { get; set; }


        /*let options = {
            'mediaConstraints': { 'audio': true, 'video': true },
            'pcConfig': {
                'iceServers': [
                    { 'urls': ["stun:stun.l.google.com:19302"] }
                ],
                'iceTransportPolicy': "all"
            }
        };
        */
    }
}
