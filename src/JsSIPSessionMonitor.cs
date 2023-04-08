using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP.Events;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPSessionMonitor : JsSIPSessionInfo, IDisposable
    {
        [JsonIgnore]
        public JsSIPSessions? Control { get; set; }


        #region JS REFERENCE

        private DotNetObjectReference<JsSIPSessionMonitor>? _reference;

        public DotNetObjectReference<JsSIPSessionMonitor> GetReference()
        {
            if (_reference == null)
                _reference = DotNetObjectReference.Create(this);
            return _reference;
        }

        #endregion

        public JsSIPSessionMonitor(string id)
        {
            this.Id = id;
        }

        public async void Terminate()
        {

        }

        #region JAVASCRIPT EVENTS


        [JSInvokable]
        public void OnProgress(JsSIPSessionEvent @event)
        {
            Console.WriteLine($"progress: {@event}");
            Status = JsSIPSessionStatus.STATUS_1XX_RECEIVED;
            NotifyChanged();
        }

        [JSInvokable]
        public void OnAccepted(JsSIPSessionEvent @event)
        {
            if (@event.Originator == JsSIPSessionEventOriginator.remote)
            {
                Status = JsSIPSessionStatus.STATUS_INVITE_SENT;
                NotifyChanged();
            }
            else
            {
                Console.WriteLine($"accepted: {@event}");
            }
        }

        [JSInvokable]
        public void OnConnecting(JsonElement @event)
        {
            Console.WriteLine($"connecting: {@event}");
        }        

        [JSInvokable]
        public void OnConfirmed(JsSIPSessionEvent @event)
        {
            if (@event.Originator == JsSIPSessionEventOriginator.local)
            {
                Status = JsSIPSessionStatus.STATUS_CONFIRMED;
                NotifyChanged();
            } else
            {
                Console.WriteLine($"confirmed: {@event}");
            }
        }

        [JSInvokable]
        public void OnEnded(JsSIPSessionEvent @event)
        {
            if (@event.Cause == "Terminated")
            {
                Status = JsSIPSessionStatus.STATUS_TERMINATED; 
                NotifyChanged();
            }
            else
            {
                Console.WriteLine($"ended: {@event}");
            }
        }
        
        #endregion

        /// <summary>
        /// Motivo da finalização da seção
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("cause")]
        public string? Cause { get; internal set; }

        public event EventHandler? OnChanged;

        private void NotifyChanged()
        {            
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
