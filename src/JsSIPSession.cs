using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP.Events;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPSession : JsSIPSessionInfo
    {
        [JsonConstructor]
        public JsSIPSession() { }

        public JsSIPSession(JsSIPSessionInfo info)
        {
            this.Id = info.Id;
            this.Direction = info.Direction;
            this.Status = info.Status;
            this.RemoteUser = info.RemoteUser;
        }

        /// <summary>
        /// Motivo da finalização da seção
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("cause")]
        [JsonConverter(typeof(EnumConverter<JsSIPSessionCause>))]
        public JsSIPSessionCause? Cause { get; internal set; }

        /// <summary>
        ///     Time stamp for the last event
        /// </summary>
        public DateTime GetTimestamp() => _events.OrderByDescending(s => s.Timestamp).Select(s => s.Timestamp).FirstOrDefault();
                
        public IEnumerable<JsSIPEvent> Events() => _events;

        [JsonIgnore]
        public List<JsSIPEvent> _events = new();

        public void Append(JsonElement jsonElement) => Append(new JsSIPEvent());

        public void Append(JsSIPEvent jsSIPEvent)
        {
            if (jsSIPEvent.Timestamp > GetTimestamp())
            {                
                switch (jsSIPEvent)
                {
                    case FailedSessionEventArgs updating:
                        {                            
                            this.Cause = updating.Cause;
                            break;
                        }
                    default: break;
                }

                NotifyChanged();
            }

            _events.Add(jsSIPEvent);
        }

        public void Append(JsSIPSessionEvent jsSIPEvent)
        {
            Cause = jsSIPEvent.Cause;

            switch (jsSIPEvent.Cause)
            {
                case JsSIPSessionCause.USER_DENIED_MEDIA_ACCESS:
                    {                        
                        break;
                    }
                default: break;
            }

            NotifyChanged();
        }

        public event EventHandler? OnChanged;

        private void NotifyChanged()
        {            
            OnChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
