using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Sufficit.Telephony.JsSIP
{
    /// <summary>
    /// Basic information about SIP session
    /// </summary>
    public class JsSIPSessionInfo
    {
        /// <summary>
        /// Identificador único
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        /// <summary>
        /// Direção da seção, Entrada ou Saída
        /// </summary>
        [JsonPropertyName("direction")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JsSIPSessionDirection Direction { get; set; }

        /// <summary>
        /// Estado atual da seção
        /// </summary>
        [JsonPropertyName("status")]
        public JsSIPSessionStatus Status { get; set; }


        [JsonPropertyName("remoteuser")]
        public string? RemoteUser { get; set; }
    }
}
