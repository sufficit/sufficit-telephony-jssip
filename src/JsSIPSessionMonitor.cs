using Microsoft.JSInterop;
using Sufficit.Notification;
using System.Text.Json.Serialization;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPSessionMonitor : JsSIPSessionControl
    {
        public JsSIPSessionStatus Status => _session.Status;

        [JsonIgnore]
        public JsSIPSessions? Control { get; set; }

        /// <summary>
        ///     Last event for this session
        /// </summary>
        public JsSIPSessionEvent? Event { get; set; }

        #region ACKNOWLEDGED

        /// <summary>
        ///     Indicates that the user has finished and acknoledge this session end status
        /// </summary>
        public bool Acknowledged { get; internal set; }

        public event EventHandler? OnAcknowledge;

        public void Acknowledge()
        {
            if (!Acknowledged)
            {
                Acknowledged = true;
                OnAcknowledge?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        private readonly JsSIPSession _session;

        public JsSIPSessionMonitor(IJSObjectReference context, JsSIPSession session) : base (session.Id, context)
        {
            _session = session;
        }

        public event EventHandler? OnChanged { add => _session.OnChanged += value; remove => _session.OnChanged -= value; }
    }
}
