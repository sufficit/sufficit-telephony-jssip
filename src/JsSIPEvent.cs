using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPEvent
    {
        /// <summary>
        /// Momento em que ocorreu o evento
        /// </summary>
        public DateTime Update { get; set; } = DateTime.UtcNow;
    }
}
