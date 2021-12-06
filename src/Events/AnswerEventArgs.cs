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
        public AnswerEventArgs()
        {
            MediaConstraints = new JsSIPMediaConstraints();
        }

        [JsonPropertyName("mediaConstraints")]
        public JsSIPMediaConstraints MediaConstraints { get; set; }


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
