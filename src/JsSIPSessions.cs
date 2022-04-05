using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPSessions : IEnumerable<JsSIPSession>
    {
        /// <summary>
        /// Informativo to prepend msg logs
        /// </summary>
        const string logPrepend = "JsSIP Blazor (Sessions),";

        public const string JsSIPScriptFile = "./_content/Sufficit.Telephony.JsSIP/jssip-sessions.min.js";

        private readonly object _lock;
        private readonly List<JsSIPSession> _sessions;
        private readonly DotNetObjectReference<JsSIPSessions> _reference;
        private readonly ILogger _logger;
        private readonly IJSRuntime _jSRuntime;
        private readonly SemaphoreSlim _semaphore;

        public event EventHandler? OnChanged;

        public JsSIPSessions(ILogger<JsSIPSessions> logger, IJSRuntime JSRuntime)
        {
            _logger = logger;
            _jSRuntime = JSRuntime;

            _lock = new object();
            _sessions = new List<JsSIPSession>();
            _reference = DotNetObjectReference.Create(this);
            _semaphore = new SemaphoreSlim(1, 1);
        }

        private IJSObjectReference? _context;
        public async Task<IJSObjectReference> JSContext()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_context == null)
                {
                    _context = await _jSRuntime.InvokeAsync<IJSObjectReference>("import", JsSIPScriptFile);
                    await _context.InvokeVoidAsync("Reference", _reference);
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return _context;
        }

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

            await (await JSContext()).InvokeVoidAsync("onJsSIPSession", session);
        }

        /// <summary>
        /// Encaminha o evento de mudança
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSessionChanged(object? sender, EventArgs e)
        {
            OnChanged?.Invoke(sender, e);
        }

        public JsSIPSession? Find(string id)
        {
            lock (_lock)
            {
                return _sessions.Find(s => s.ID == id);
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

        #region EVENTS FROM JAVASCRIPT

        [JSInvokable]
        public async Task onPeerconnection(JsSIPSession info, JsonElement args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onPeerconnection, session id: { info.ID }, args: { args.GetRawText() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on peer connection"); }
        }

        [JSInvokable]
        public async Task onConnecting(JsSIPSession info, JsonElement args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onConnecting, session id: { info.ID }, args: { args.GetRawText() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on connecting"); }
        }

        [JSInvokable]
        public async Task onAccepted(JsSIPSession info, JsonElement args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onAccepted, session id: { info.ID }, args: { args.GetRawText() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on accepted"); }
        }

        [JSInvokable]
        public async Task onFailed(JsSIPSession info, FailedSessionEventArgs args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onFailed, session id: { info.ID }, args: { args.ToJsonString() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);
            
            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on failed"); }
        }

        [JSInvokable]
        public async Task onEnded(JsSIPSession info, FailedSessionEventArgs args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onEnded, session id: { info.ID }, args: { args.ToJsonString() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on ended"); }
        }

        [JSInvokable]
        public async Task onConfirmed(JsSIPSession info, FailedSessionEventArgs args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onConfirmed, session id: { info.ID }, args: { args.ToJsonString() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on confirmed"); }
        }

        [JSInvokable]
        public async Task onMuted(JsSIPSession info, JsonElement args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onMuted, session id: { info.ID }, args: { args.GetRawText() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on muted"); }
        }

        [JSInvokable]
        public async Task onUnmuted(JsSIPSession info, JsonElement args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onUnmuted, session id: { info.ID }, args: { args.GetRawText() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on unmuted"); }
        }

        [JSInvokable]
        public async Task onHold(JsSIPSession info, JsonElement args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onHold, session id: { info.ID }, args: { args.GetRawText() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on hold"); }
        }

        [JSInvokable]
        public async Task onUnhold(JsSIPSession info, JsonElement args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onUnhold, session id: { info.ID }, args: { args.GetRawText() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on unhold"); }
        }

        [JSInvokable]
        public async Task onProgress(JsSIPSession info, JsonElement args)
        {
            await Task.Yield();
            _logger?.LogDebug($"onProgress, session id: { info.ID }, args: { args.GetRawText() }");

            JsSIPSession? session = null;
            lock (_lock) session = _sessions.Find(s => s.ID == info.ID);

            if (session != null)
            {
                session.Status = info.Status;
                session.Append(args);
            }
            else { throw new System.Exception("session not found on progress"); }
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
            _logger?.LogTrace($"{logPrepend} JsSIPService Action({DateTime.Now}): SessionID: {session.ID}");
            await (await JSContext()).InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "Papayas");
        }

        public async Task Answer(JsSIPSessionInfo session, bool video = false)
        {
            _logger?.LogTrace($"{logPrepend} JsSIPService Answer({DateTime.Now}): SessionID: {session.ID}, Video: {video}");

            var arguments = new AnswerEventArgs();

            if (arguments.MediaConstraints == null) arguments.MediaConstraints = new MediaConstraints() { Video = video };
            else arguments.MediaConstraints.Video = video;

            await (await JSContext()).InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "answer", arguments);
        }

        public async Task Originate(string uri, MediaConstraints mediaConstraints)
        {
            _logger?.LogTrace($"{logPrepend} JsSIPService Originate({DateTime.Now}): Uri: {uri}");
            var arguments = new AnswerEventArgs();
            arguments.MediaConstraints = mediaConstraints;

            await (await JSContext()).InvokeVoidAsync("Originate", uri, arguments);
        }

        public async Task Terminate(JsSIPSessionInfo session)
        {
            _logger?.LogTrace($"{logPrepend} JsSIPService Terminate({DateTime.Now}): SessionID: {session.ID}");
            await (await JSContext()).InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "terminate");
        }

        public async Task Mute(JsSIPSessionInfo session, bool audio = true, bool video = true)
        {
            _logger?.LogTrace($"{logPrepend} JsSIPService Mute({DateTime.Now}): SessionID: {session.ID}");

            IMediaBasic arguments = new MediaConstraints() { Audio = audio, Video = video };
            await (await JSContext()).InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "mute", arguments);
        }

        public async Task UnMute(JsSIPSessionInfo session, bool audio = true, bool video = true)
        {
            _logger?.LogTrace($"{logPrepend} JsSIPService UnMute({DateTime.Now}): SessionID: {session.ID}");

            IMediaBasic arguments = new MediaConstraints() { Audio = audio, Video = video };
            await (await JSContext()).InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "unmute", arguments);
        }

        public async Task Hold(JsSIPSessionInfo session)
        {
            _logger?.LogTrace($"{logPrepend} JsSIPService Hold({DateTime.Now}): SessionID: {session.ID}");
            await (await JSContext()).InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "hold");
        }

        public async Task UnHold(JsSIPSessionInfo session)
        {
            _logger?.LogTrace($"{logPrepend} JsSIPService UnHold({DateTime.Now}): SessionID: {session.ID}");
            await (await JSContext()).InvokeVoidAsync(JSSIPSESSIONACTIONSFUNC, session, "unhold");
        }

        #endregion
    }
}
