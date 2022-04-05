using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPOptions
    {
        public const string SectionName = "Sufficit:JsSIP";
        public JsSIPOptions()
        {
            this.Uri = string.Empty;
            this.Password = string.Empty;  
            this.UserAgent = "Sufficit WebRTC Phone";
            this.Sockets = new string[]{ };
            this.TraceSip = false;
            this.StunServers = new string[] { };
        }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("user_agent")]
        public string UserAgent { get; set; }

        [JsonPropertyName("sockets")]
        public string[] Sockets { get; set; }

        // Optional

        [JsonPropertyName("traceSip")]
        public bool TraceSip { get; set; }

        [JsonPropertyName("stunServers")]
        public string[] StunServers { get; set; }

        /*
         
         // optional configurations
    authorizationUser: null,
    register: true,
    rel100: 'supported',
    registerExpires: null,
    noAnswerTimeout: null,
    turnServers: null,
    usePreloadedRoute: null,
    connectionRecoveryMinInterval: null,
    connectionRecoveryMaxInterval: null,
    hackViaTcp: null,
    hackIpInContact: null,  
        
        */
    }
}
