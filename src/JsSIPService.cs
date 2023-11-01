using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP.Accounts;
using Sufficit.Telephony.JsSIP.Methods;
using System;
using System.ComponentModel;
using System.Text.Json;
using System.Threading;
using static Sufficit.Telephony.JsSIP.JsSIPGlobals;


namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPService : JsSIPContextRuntime
    {
        /// <summary>
        /// Informativo to prepend msg logs
        /// </summary>
        const string logPrepend = "JsSIP Blazor (Service),";

        const string JsSIPFile = "jssip-3.10.0.min.js";
        const string JsSIPFullPath = $"./_content/{JsSIPNamespace}/{JsSIPFile}";

        private readonly ILogger _logger;
        private readonly List<string> _jsLogs;
        private JsSIPOptions _options;

#if DEBUG
        protected override string JsSIPScriptFile { get; } = $"./_content/{JsSIPNamespace}/jssip-service.js";
#else
        protected override string JsSIPScriptFile { get; } = $"./_content/{JsSIPNamespace}/jssip-service.min.js";
#endif

        public JsSIPService(IOptions<JsSIPOptions> options, JsSIPSessions sessions, ILogger<JsSIPService> logger, IJSRuntime JSRuntime) : base (JSRuntime)
        {
            _options = options.Value;
            _logger = logger;

            Monitor = new JsSIPMonitor();

            Account = new JsSIPAccount();
            Account.OnChanged += OnAccountChanged;

            Sessions = sessions;
            Sessions.Monitor = Monitor;
            Sessions.OnChanged += NotifyChanged;

            _jsLogs = new List<string>();
            Status = string.Empty;

            Devices = new MediaDeviceGroup();
            Devices.OnChanged += OnMediaDeviceChanged;
        }

        protected override async ValueTask OnReady()
        {
            var accountReference = DotNetObjectReference.Create(Account);
            var serviceReference = DotNetObjectReference.Create(this);
            await InvokeVoidAsync("Reference", JsSIPFullPath, serviceReference, accountReference);
        }
         
        /// <summary>
        /// Is WebSocket Connected, its only indicates that the server is ok
        /// </summary>
        public bool IsConnected { get; protected set; }

        public event EventHandler? OnChanged;

        public JsSIPSessions Sessions { get; }

        /// <summary>
        /// Last knowing status, after a success status changed
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Default media devices to use
        /// </summary>
        public MediaDeviceGroup Devices { get; }

        /// <summary>
        /// Default sip account
        /// </summary>
        public JsSIPAccount Account { get; }

        private void OnAccountChanged(object? sender, AccountChangedEventArgs e)
        {
            _logger.LogDebug("account changed: {status}", e.Event);
            NotifyChanged();
        }

        public async Task<string> GetStatus()
        {
            var socketStatus = await GetWebSocketStatus();
            if (socketStatus == JsSIPWebSocketStatus.STATUS_READY)
            {
                if (Account.IsRegistered)
                    return "SOCKET READY AND REGISTERED";
                else
                    return "SOCKET READY BUT UNREGISTERED";                
            }
            return "SOCKET NOT CONNECTED";
        }

        private void NotifyChanged() => NotifyChanged(this, EventArgs.Empty);
        private async void NotifyChanged(object? sender, EventArgs e)
        {
            Status = await GetStatus();
            OnChanged?.Invoke(this, EventArgs.Empty);
        }


        public IEnumerable<string> JSLogs => _jsLogs;

        public async Task<JsSIPWebSocketStatus> GetWebSocketStatus()
        {
            var response = await InvokeAsync<int>("GetStatus");
            return (JsSIPWebSocketStatus)response;
        }

        /// <summary>
        ///     Request Browser Media Access
        /// </summary>
        public async Task RequestMediaAccess()
        {
            await InvokeVoidAsync(nameof(RequestMediaAccess));
        }

        /// <summary>
        ///     Get JsSIP Version
        /// </summary>
        public async Task<string> GetVersion()
        {
            return await InvokeAsync<string>(nameof(GetVersion));
        }

        #region EVENTS CALLS FROM JAVASCRIPT

        /// <summary>
        ///     Invoked on Javascript Console event like log, debug, info, error, etc
        /// </summary>
        [JSInvokable]
        public async Task onConsoleEvent(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _jsLogs.Add(args.GetRawText());
                NotifyChanged();
            }
        }

        [JSInvokable]
        public async Task onDependenciesLoaded(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                string? name = args.GetProperty("name").GetString();
                string? version = args.GetProperty("version").GetString();
                _logger?.LogDebug($"{logPrepend} HandleJsSIPLoad: {name}({version})");

                // await _jSRuntime.InvokeVoidAsync("interceptConsoleEvents", _reference);
                //if(string.IsNullOrWhiteSpace(Status))
                    //await Start();
            }
        }

        /// <summary>
        ///     Start service 
        /// </summary>
        public async Task Start(JsSIPOptions? options = null)
        {
            if (options != null) _options = options;

            if (!_options.Sockets.Any())
                throw new Exception("none valid socket configured");

            await InvokeVoidAsync("onJsSIPLoaded", _options);
            NotifyChanged();
        }

        [JSInvokable]
        public async Task onEvent(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger?.LogTrace($"{ logPrepend } onEvent: { args.GetRawText() }");
                _logger?.LogInformation($"{ logPrepend } Unknown event");
                NotifyChanged();
            }
        }

        [JSInvokable]
        public async Task onConnected(JsonElement args)
        {
            IsConnected = true;

            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger?.LogTrace($"{ logPrepend } onConnected: { args.GetRawText() }");
                _logger?.LogInformation($"{ logPrepend } connected event");
                NotifyChanged();
            }
        }

        [JSInvokable]
        public async Task onDisconnected(JsonElement args)
        {
            IsConnected = false;

            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger?.LogTrace($"{ logPrepend } onDisconnected: { args.GetRawText() }");
                _logger?.LogInformation($"{ logPrepend } disconnected event");

                NotifyChanged();
            }
        }

        [JSInvokable]
        public async Task onNewRTCSession(JsSIPSessionInfo info)
        {
            _logger?.LogInformation($"{ logPrepend } new rtc session event, id: ({info.Id}), for: ({info.RemoteUser})");

            var session = new JsSIPSession(info);
            await Sessions.Append(session);            
        }

        [JSInvokable]
        public async Task onNewMessage(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger?.LogTrace($"{ logPrepend } onNewMessage: { args.GetRawText() }");
                _logger?.LogInformation($"{ logPrepend } new message event");

                NotifyChanged();
            }
        }

        /*
         
        [JSInvokable]
        async void onRegistered(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger.LogTrace($"{ logPrepend } onRegistered: { args.GetRawText() }");
                _logger.LogInformation($"{ logPrepend } registered event");

                NotifyChanged();
            }
        }

        [JSInvokable]
        async void onUnregistered(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger.LogDebug($"{ logPrepend } onUnregistered: { args.GetRawText() }");
                _logger.LogInformation($"{ logPrepend } unregistered event");

                NotifyChanged();
            }
        }


        [JSInvokable]
        public async Task onRegistrationFailed(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger.LogDebug($"{ logPrepend } onRegistrationFailed: { args.GetRawText() }");
                _logger.LogInformation($"{ logPrepend } registration failed event");

                NotifyChanged();
            }
        }

        */

        [JSInvokable]
        public async Task onRinging(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger?.LogTrace($"{ logPrepend } onRinging: { args.GetRawText() }");
                _logger?.LogInformation($"{ logPrepend } ringing event");

                NotifyChanged();
            }
        }

        [JSInvokable]
        public async Task onAck(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger?.LogTrace($"{ logPrepend } onAck: { args.GetRawText() }");
                _logger?.LogInformation($"{ logPrepend } ack event");

                NotifyChanged();
            }
        }

        #endregion
        
        /// <summary>
        /// Realiza uma chamada
        /// </summary>
        public async Task<JsSIPSessionInfo> Call(string uri, bool video = true)
        {
            var mediaConstraints = new MediaConstraints();
            if (!string.IsNullOrWhiteSpace(Devices.AudioInput))
                mediaConstraints.Audio = new MediaOptions() { DeviceID = Devices.AudioInput };

            if (video) {
                if (!string.IsNullOrWhiteSpace(Devices.VideoInput))
                    mediaConstraints.Video = new MediaOptions() { DeviceID = Devices.VideoInput };
                else mediaConstraints.Video = new MediaOptions(true);
            }

            var json = JsonSerializer.Serialize(mediaConstraints);
            _logger.LogInformation("call: {0}", json);

            return await Sessions.Originate(uri, mediaConstraints);
        }

        public async Task<JsSIPSessionMonitor> CallMonitor(string uri, bool video = true)
        {
            var info = await Call(uri, video);
            return Sessions.CallMonitor(info);
        }

        /// <summary>
        ///     Recupera os dispositivos de media disponíveis
        /// </summary>
        public async Task<IEnumerable<JsSIPMediaDevice>> GetMediaDevices()
        {
            return await InvokeAsync<IEnumerable<JsSIPMediaDevice>>(nameof(GetMediaDevices));
        }

        public async Task<TestDevicesResponse> TestDevices(TestDevicesRequest request)
        {
            return await InvokeAsync<TestDevicesResponse>(nameof(TestDevices), request);           
        }

        private async void OnMediaDeviceChanged(object? sender, EventArgs e)
        {
            if (sender is MediaDeviceGroup group)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(group.VideoInput))
                    {
                        await InvokeVoidAsync("MediaDeviceUpdate", "videoinput", group.VideoInput);
                    }

                    if (!string.IsNullOrWhiteSpace(group.AudioOuput))
                    {
                        await InvokeVoidAsync("MediaDeviceUpdate", "audiooutput", group.AudioOuput);
                    }
                }
                catch(Exception ex)
                {
                    _logger?.LogError($"error after devices changed: {ex.Message}", ex);
                }
            }
        }

        public JsSIPMonitor Monitor { get; }

        /// <summary>
        ///     Recover a session control object
        /// </summary>
        public JsSIPSessionControl GetControl(string id)
            => new JsSIPSessionControl(id, Sessions);
    }
}
