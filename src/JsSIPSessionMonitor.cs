using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP.Events;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPSessionMonitor : JsSIPSessionInfo, IDisposable
    {
        [JsonIgnore]
        public JsSIPSessions? Control { get; set; }

        /// <summary>
        /// Javascript context
        /// </summary>
        private IJSObjectReference _context;

        #region JS REFERENCE

        private DotNetObjectReference<JsSIPSessionMonitor>? _reference;

        public DotNetObjectReference<JsSIPSessionMonitor> GetReference()
        {
            if (_reference == null)
                _reference = DotNetObjectReference.Create(this);
            return _reference;
        }

        #endregion

        public JsSIPSessionMonitor(IJSObjectReference context, string id)
        {
            this.Id = id;
            _context = context;
        }

        public async Task<JsSIPSessionMonitor> Initialize()
        {
            await _context.InvokeVoidAsync("Monitor", Id, GetReference());
            return this;
        }

        public async Task Terminate()
        {
            await _context.InvokeVoidAsync("Terminate", Id);
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
        public void OnConnecting(JsSIPSessionEvent @event)
        {
            if (@event.Originator == JsSIPSessionEventOriginator.remote)
            {
                Status = JsSIPSessionStatus.STATUS_CANCELED;
                Cause = @event.Cause.ToString();
                NotifyChanged();
            }
            else
            {
                Console.WriteLine($"connecting: {@event}");
            }
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
        public void OnFailed(JsonElement @event)
        {
            Console.WriteLine($"connecting: {@event}");
            
            Status = JsSIPSessionStatus.STATUS_CANCELED;
            NotifyChanged();
        }
        

        [JSInvokable]
        public void OnEnded(JsSIPSessionEvent @event)
        {
            if (@event.Cause == JsSIPSessionCause.BYE)
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

        public delegate ValueTask AsyncEventHandler(JsSIPSessionMonitor sender);

        public event AsyncEventHandler? OnChanged;

        private void NotifyChanged()
        {            
            OnChanged?.Invoke(this);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
