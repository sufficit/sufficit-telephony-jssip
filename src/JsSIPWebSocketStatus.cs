using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public enum JsSIPWebSocketStatus
    {
        CONFIGURATION_ERROR = -1,
        NETWORK_ERROR = -2,
        STATUS_INIT = 0,
        STATUS_NOT_READY = 3,
        STATUS_READY = 1,
        STATUS_USER_CLOSED = 2
    }
}
