using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP.Accounts
{
    public class AccountChangedEventArgs
    {
        public AccountChangedEvent Event { get; set; }
    }

    [Flags]
    public enum AccountChangedEvent
    {
        RegistrationChanged,
        RegistrationStatusChanged,
    }
}
