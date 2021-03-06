using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Sufficit.Telephony.JsSIP.Events;
using Sufficit.Telephony.JsSIP.Extensions;
using System;
using System.Text.Json;

namespace Sufficit.Telephony.JsSIP
{
    public class JsSIPService 
    {
        /// <summary>
        /// Informativo to prepend msg logs
        /// </summary>
        const string logPrepend = "JsSIP Blazor (Service),";

        public const string JsSIPBaseFile = "./_content/Sufficit.Telephony.JsSIP/jssip-3.9.0.min.js";
        public const string JsSIPScriptFile = "./_content/Sufficit.Telephony.JsSIP/jssip-service.min.js";

        private readonly ILogger _logger;
        private readonly IJSRuntime _jSRuntime;
        private readonly List<string> _jsLogs;
        private readonly DotNetObjectReference<JsSIPService> _reference; 
        private readonly SemaphoreSlim _semaphore;
        private JsSIPOptions _options;

        private IJSObjectReference? _context;
        public async Task<IJSObjectReference> JSContext()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_context == null)
                {
                    _context = await _jSRuntime.InvokeAsync<IJSObjectReference>("import", JsSIPScriptFile);
                    await _context.InvokeVoidAsync("Reference", JsSIPBaseFile, _reference);
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return _context;
        }

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

        public JsSIPService(IOptions<JsSIPOptions> options, JsSIPSessions sessions, ILogger<JsSIPService> logger, IJSRuntime JSRuntime)
        {
            _options = options.Value;
            _logger = logger;
            _jSRuntime = JSRuntime;

            Sessions = sessions;
            Sessions.OnChanged += NotifyChanged;

            _semaphore = new SemaphoreSlim(1, 1);
            _jsLogs = new List<string>();
            _reference = DotNetObjectReference.Create(this);
            Status = string.Empty;

            Devices = new MediaDeviceGroup();
            Devices.OnChanged += Devices_OnChanged;
        }

        private void NotifyChanged() => NotifyChanged(this, EventArgs.Empty);
        private async void NotifyChanged(object? sender, EventArgs e)
        {
            Status = (await GetStatus()).ToString();
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerable<string> JSLogs => _jsLogs;


        public async Task<JsSIPServiceStatus> GetStatus()
        {
            var response = await (await JSContext()).InvokeAsync<int>("GetStatus");
            return (JsSIPServiceStatus)response;
        }

        #region EVENTS CALLS FROM JAVASCRIPT

        /// <summary>
        /// Invoked on Javascript Console event like log, debug, info, error, etc
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
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
        /// Start service 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task Start(JsSIPOptions? options = null)
        {
            if (options != null) _options = options;

            if (!_options.Sockets.Any())
                throw new Exception("none valid socket configured");

            await (await JSContext()).InvokeVoidAsync("onJsSIPLoaded", _options);
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
            _logger?.LogInformation($"{ logPrepend } new rtc session event, id: ({ info.ID })");

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

        [JSInvokable]
        public async Task onRegistered(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger?.LogTrace($"{ logPrepend } onRegistered: { args.GetRawText() }");
                _logger?.LogInformation($"{ logPrepend } registered event");

                NotifyChanged();
            }
        }

        [JSInvokable]
        public async Task onUnregistered(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger?.LogTrace($"{ logPrepend } onUnregistered: { args.GetRawText() }");
                _logger?.LogInformation($"{ logPrepend } unregistered event");

                NotifyChanged();
            }
        }

        [JSInvokable]
        public async Task onRegistrationFailed(JsonElement args)
        {
            await Task.Yield();
            if (args.ValueKind != JsonValueKind.Undefined && args.ValueKind != JsonValueKind.Null)
            {
                _logger?.LogTrace($"{ logPrepend } onRegistrationFailed: { args.GetRawText() }");
                _logger?.LogInformation($"{ logPrepend } registration failed event");

                NotifyChanged();
            }
        }

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
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task Call(string uri, bool video = true)
        {
            var mediaConstraints = new MediaConstraints();
            if(!string.IsNullOrWhiteSpace(Devices.AudioInput))
                mediaConstraints.Audio = new MediaOptions() { DeviceID = Devices.AudioInput };

            if (video) {
                if (!string.IsNullOrWhiteSpace(Devices.VideoInput))
                    mediaConstraints.Video = new MediaOptions() { DeviceID = Devices.VideoInput };
                else mediaConstraints.Video = new MediaOptions(true);
            }

            await Sessions.Originate(uri, mediaConstraints);
        }

        /// <summary>
        /// Recupera os dispositivos de media disponíveis
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<JsSIPMediaDevice>> MediaDevices()
        {
            return await (await JSContext()).InvokeAsync<IEnumerable<JsSIPMediaDevice>>("MediaDevices");
        }

        public async Task TestDevices()
        {
            await (await JSContext()).InvokeVoidAsync("TestDevices");            
        }

        private async void Devices_OnChanged(object? sender, EventArgs e)
        {
            if(sender != null && sender is MediaDeviceGroup group)
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(group.VideoInput))
                    {
                        await (await JSContext()).InvokeVoidAsync("MediaDeviceUpdate", "videoinput", group.VideoInput);
                    }

                    if (!string.IsNullOrWhiteSpace(group.AudioOuput))
                    {
                        await (await JSContext()).InvokeVoidAsync("MediaDeviceUpdate", "audiooutput", group.AudioOuput);
                    }
                }
                catch(Exception ex)
                {
                    _logger?.LogError($"error after devices changed: {ex.Message}", ex);
                }
            }
        }
    }
}
