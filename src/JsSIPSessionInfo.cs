using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    /// <summary>
    /// Basic information about SIP session
    /// </summary>
    public class JsSIPSessionInfo
    {
        public JsSIPSessionInfo()
        {
            ID = string.Empty;
            Direction = string.Empty;
            Status = JsSIPSessionStatus.STATUS_NULL;
        }

        /// <summary>
        /// Identificador único
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Direção da seção, Entrada ou Saída
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// Estado atual da seção
        /// </summary>
        public JsSIPSessionStatus Status { get; set; }
    }
}
