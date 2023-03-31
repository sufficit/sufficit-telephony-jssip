using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP.SIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP.Accounts
{
    public class JsSIPAccount
    {
        /// <summary>
        /// Is SIP registered
        /// </summary>
        public bool IsRegistered { get; internal set; }

        /// <summary>
        /// Register expiration
        /// </summary>
        public DateTime Expiration { get; internal set; }

        /// <summary>
        /// Last registration event information
        /// </summary>
        public (DateTime, string)? LastRegistration { get; internal set; }


        public event EventHandler<AccountChangedEventArgs>? OnChanged;


        [JSInvokable]
        public void NotifyRegistered(JsSIPEvent @event)
        {
            LastRegistration = (DateTime.UtcNow, "registered");
            var expirationValue = @event.Response.GetProperty("headers").GetProperty("Expires")[0].GetProperty("raw").GetString();
            if (int.TryParse(expirationValue, out int expiration))
                Expiration = DateTime.UtcNow.AddSeconds(expiration);

            Console.WriteLine($"onRegistered: valid until => {Expiration:o}");

            var args = new AccountChangedEventArgs();
            args.Event = AccountChangedEvent.RegistrationChanged;

            if (!IsRegistered)
            {
                IsRegistered = true;
                args.Event |= AccountChangedEvent.RegistrationStatusChanged;
            }

            OnChanged?.Invoke(this, args);
        }

        [JSInvokable]
        public void NotifyUnregistered(JsonElement _)
        {
            LastRegistration = (DateTime.UtcNow, "unregistered");
            Console.WriteLine("onUnregistered");

            var args = new AccountChangedEventArgs();
            args.Event = AccountChangedEvent.RegistrationChanged;

            if (IsRegistered)
            {
                IsRegistered = false;
                args.Event |= AccountChangedEvent.RegistrationStatusChanged;
            }

            OnChanged?.Invoke(this, args);
        }

        [JSInvokable]
        public void NotifyRegistrationFailed(JsonElement @event)
        {
            Console.WriteLine($"onRegistrationFailed: {@event.GetRawText()}");
            LastRegistration = (DateTime.UtcNow, "fail");

            var args = new AccountChangedEventArgs();
            args.Event = AccountChangedEvent.RegistrationChanged;

            if (IsRegistered && Expiration < DateTime.UtcNow)
            {
                IsRegistered = false;                
                args.Event |= AccountChangedEvent.RegistrationStatusChanged;
            }

            OnChanged?.Invoke(this, args);
        }
    }
}
