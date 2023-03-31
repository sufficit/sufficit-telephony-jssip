using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPEvent
    {
        /// <summary>
        /// Momento em que ocorreu o evento
        /// </summary>
        public DateTime Update { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("response")]
        public virtual JsonElement Response { get; set; }
    }
}
