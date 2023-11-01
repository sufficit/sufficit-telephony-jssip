using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using static Sufficit.Telephony.JsSIP.JsSIPGlobals;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPSessions : JsSIPContextRuntime, IEnumerable<JsSIPSession>
    {
        /// <summary>
        /// Informativo to prepend msg logs
        /// </summary>
        const string logPrepend = "JsSIP Blazor (Sessions),";

#if DEBUG
        protected override string JsSIPScriptFile { get; } = $"./_content/{JsSIPNamespace}/jssip-sessions.js";
#else
        protected override string JsSIPScriptFile { get; } = $"./_content/{JsSIPNamespace}/jssip-sessions.min.js";
#endif

        private readonly object _lock;
        private readonly List<JsSIPSession> _sessions;
        private readonly DotNetObjectReference<JsSIPSessions> _reference;
        private readonly ILogger _logger;

        public event EventHandler? OnChanged;

        public JsSIPSessions(ILogger<JsSIPSessions> logger, IJSRuntime JSRuntime) : base (JSRuntime)
        {
            _logger = logger;

            _lock = new object();
            _sessions = new List<JsSIPSession>();
            _reference = DotNetObjectReference.Create(this);

            // setting default monitor
            Monitor = new JsSIPMonitor();
        }

        protected override ValueTask OnReady()
            => InvokeVoidAsync("Reference", _reference);

        public async Task Append(JsSIPSession session)
        {
            lock (_lock)
            {
                if (!_sessions.Contains(session))
                {
                    // vinculando o monitor de eventos
                    session.OnChanged += onSessionChanged;

                    _sessions.Add(session);

                    // alertando atualizações 
                    OnChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            // Gerando o evento de atualização
            //session.Append(new JsSIPEvent());
            // if (session.Direction != "inbound") return;

            await AttachEventHandlers(session);
        }


        /// <summary>
        /// Starts to monitor a sip session
        /// </summary>
        public JsSIPSessionMonitor CallMonitor(JsSIPSessionInfo info)
        {
            var session = Find(info.Id);
            if (session != null)
            {
                var monitor = new JsSIPSessionMonitor(this, session);
                //await monitor.Initialize();
                return monitor;
            }
            else { throw new System.Exception("session not found on monitor"); }
            
        }

        public ValueTask<JsSIPSessionInfo> GetSession(string id)
            => InvokeAsync<JsSIPSessionInfo>(nameof(GetSession), id);

        public async ValueTask<JsSIPSessionInfo?> TryGetSession(string? id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    return await InvokeAsync<JsSIPSessionInfo>(nameof(GetSession), id);
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// Encaminha o evento de mudança
        /// </summary>
        private void onSessionChanged(object? sender, EventArgs e)
        {
            OnChanged?.Invoke(sender, e);
        }

        public JsSIPSession? Find(string id)
        {
            lock (_lock)
            {
                return _sessions.FirstOrDefault(s => s.Id == id);
            }
        }

        #region IMPLEMENT IENUMERABLE

        public IEnumerator<JsSIPSession> GetEnumerator()
        {
            lock (_lock)
                return _sessions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion


        /// <summary>
        ///     Javascript knowing session events
        /// </summary>
        static string[] SessionEvents = {
            //"newDTMF",
            //"newInfo",
            "hold",
            "muted",
            "unhold",
            "unmuted",
            "progress",
            "succeeded",
            "failed",
            "ended",
            "confirmed",
            "connecting",
            "accepted",
            "peerconnection"
        };

        /// <summary>
        ///     Command that attaches event handlers from javascript
        /// </summary>
        public async Task AttachEventHandlers(JsSIPSession session)
        {
            await InvokeVoidAsync(nameof(AttachEventHandlers), session.Id, SessionEvents);
        }

        #region EVENTS FROM JAVASCRIPT

        #region ON HOLD

        [JSInvokable]
        public void OnHold(JsSIPSession info, JsonElement args)
        {
            _logger.LogDebug($"onHold, session id: {info.Id}, args: {args.GetRawText()}");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on hold"); }
        }

        #endregion
        #region ON MUTED

        [JSInvokable]
        public void OnMuted(JsSIPSession info, JsonElement args)
        {
            _logger.LogDebug($"onMuted, session id: {info.Id}, args: {args.GetRawText()}");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on muted"); }
        }

        #endregion
        #region ON UNHOLD

        [JSInvokable]
        public void OnUnhold(JsSIPSession info, JsonElement args)
        {
            _logger.LogDebug($"onUnhold, session id: {info.Id}, args: {args.GetRawText()}");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on unhold"); }
        }

        #endregion
        #region ON UNMUTED

        [JSInvokable]
        public void OnUnmuted(JsSIPSession info, JsonElement args)
        {
            _logger.LogDebug($"onUnmuted, session id: {info.Id}, args: {args.GetRawText()}");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on unmuted"); }
        }

        #endregion
        #region ON PEERCONNECTION

        [JSInvokable]
        public void OnPeerconnection(JsSIPSession info, JsonElement args)
        {
            _logger.LogWarning($"OnPeerconnection, session id: {info.Id}, args: { args.GetRawText() }");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on peer connection"); }
        }

        #endregion
        #region ON FAILED

        [JSInvokable]
        public void OnFailed(JsSIPSession info, JsSIPSessionEvent args)
        {
            _logger.LogWarning($"onFailed, session id: {info.Id}, args: {args.ToJsonString()}");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on failed"); }
        }

        #endregion
        #region ON PROGRESS

        [JSInvokable]
        public void OnProgress(JsSIPSession info, JsonElement args)
        {
            _logger.LogWarning($"onProgress, session id: {info.Id}, args: {args.GetRawText()}");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on progress"); }
        }

        #endregion

        [JSInvokable]
        public void OnConnecting(JsSIPSession info, JsonElement args)
        {
            _logger.LogWarning($"onConnecting, session id: {info.Id}, args: { args.GetRawText() }");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on connecting"); }
        }

        [JSInvokable]
        public void OnAccepted(JsSIPSession info, JsonElement args)
        {
            _logger.LogWarning($"onAccepted, session id: { info.Id}, args: { args.GetRawText() }");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on accepted"); }
        }

        [JSInvokable]
        public void OnEnded(JsSIPSession info, JsonElement args)
        {
            _logger.LogWarning($"onEnded, session id: { info.Id}, args: {args.GetRawText()}");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on ended"); }
        }

        [JSInvokable]
        public void OnConfirmed(JsSIPSession info, JsonElement args)
        {
            _logger.LogWarning($"onConfirmed, session id: { info.Id}, args: {args.GetRawText()}");

            var session = Find(info.Id);
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on confirmed"); }
        }

        #endregion

        #region SESSION ACTION EVENTS

        const string JSSIPSESSIONACTIONSFUNC = "JsSIPSessionActions";

        /// <summary>
        /// Teste, nenhum uso por enquanto
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task Action(JsSIPSessionInfo session)
        {
            _logger.LogTrace($"{logPrepend} JsSIPService Action({DateTime.Now}): SessionID: {session.Id}");
            await InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "Papayas");
        }

        public async Task<JsSIPSessionInfo> Originate(string uri, MediaConstraints mediaConstraints)
        {
            _logger.LogTrace($"{logPrepend} JsSIPService Originate({DateTime.Now}): Uri: {uri}");
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentNullException("uri");

            var arguments = new AnswerEventArgs();
            arguments.MediaConstraints = mediaConstraints;

            return await InvokeAsync<JsSIPSessionInfo>("Originate", uri, arguments);           
        }
         
        public async Task Mute(JsSIPSessionInfo session, bool audio = true, bool video = true)
        {
            _logger.LogTrace($"{logPrepend} JsSIPService Mute({DateTime.Now}): SessionID: {session.Id}");

            IMediaBasic arguments = new MediaConstraints() { Audio = audio, Video = video };
            await InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "mute", arguments);
        }

        public async Task UnMute(JsSIPSessionInfo session, bool audio = true, bool video = true)
        {
            _logger.LogTrace($"{logPrepend} JsSIPService UnMute({DateTime.Now}): SessionID: {session.Id}");

            IMediaBasic arguments = new MediaConstraints() { Audio = audio, Video = video };
            await InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "unmute", arguments);
        }

        public async Task Hold(JsSIPSessionInfo session)
        {
            _logger.LogTrace($"{logPrepend} JsSIPService Hold({DateTime.Now}): SessionID: {session.Id}");
            await InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "hold");
        }

        public async Task UnHold(JsSIPSessionInfo session)
        {
            _logger.LogTrace($"{logPrepend} JsSIPService UnHold({DateTime.Now}): SessionID: {session.Id}");
            await InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "unhold");
        }

        #endregion

        public JsSIPMonitor Monitor { get; set; }
    }
}
