using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    /// <summary>
    /// Parametros sobre o tipo de media a ser utilizado
    /// </summary>
    public class JsSIPMediaConstraints
    {
        [JsonPropertyName("audio")]
        public bool Audio { get; set; } = true;

        [JsonPropertyName("video")]
        public bool Video { get; set; } = true;
    }
}
