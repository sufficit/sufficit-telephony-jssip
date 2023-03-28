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
        public JsSIPSession()
        {

        }

        public JsSIPSession(JsSIPSessionInfo info) : this()
        {
            this.ID = info.ID;
            this.Direction = info.Direction;
            this.Status = info.Status;
        }

        /// <summary>
        /// Motivo da finalização da seção
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("cause")]
        public string? Cause { get; internal set; }

        /// <summary>
        /// Momento da ultima atualização
        /// </summary>
        public DateTime Update() => _events.OrderByDescending(s => s.Update).Select(p => p.Update).FirstOrDefault();

        public IEnumerable<JsSIPEvent> Events() => _events;
        public List<JsSIPEvent> _events = new();

        public void Append(JsonElement jsonElement) => Append(new JsSIPEvent());

        public void Append(JsSIPEvent jsSIPEvent)
        {
            if(jsSIPEvent.Update > Update())
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


        public event EventHandler? OnChanged;

        private void NotifyChanged()
        {            
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task Action()
        {
            await Task.Yield();
            Console.WriteLine($"JsSIPSession Action({DateTime.Now})");
        }
    }
}
