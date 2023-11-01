using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    /// <summary>
    ///     SIP Session control object
    /// </summary>
    public class JsSIPSessionControl
    {
        /// <inheritdoc cref="JsSIPSessionInfo.Id" />
        public virtual string Id { get; }

        /// <summary>
        /// Javascript context
        /// </summary>
        private readonly IJSObjectReference _context;

        public JsSIPSessionControl(string id, IJSObjectReference context) 
        {
            Id = id;
            _context = context;
        }

        /// <summary>
        ///     Answer the session, with video option
        /// </summary>
        public ValueTask Answer(bool video = false)
        {
            var arguments = new AnswerEventArgs();
            arguments.MediaConstraints ??= new MediaConstraints();
            arguments.MediaConstraints.Video = video;

            return _context.InvokeVoidAsync(nameof(Answer), Id, arguments);
        }

        /// <summary>
        ///     Terminate a call session (deny, hangup, refuse)
        /// </summary>
        public ValueTask Terminate()
            => _context.InvokeVoidAsync(nameof(Terminate), Id);
    }
}
