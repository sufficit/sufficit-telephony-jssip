using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public abstract class JsSIPContextRuntime : IJSObjectReference
    {
        public JsSIPContextRuntime(IJSRuntime JSRuntime)
        {
             _jSRuntime = JSRuntime;
            _semaphore = new SemaphoreSlim(1, 1);
        }

        private readonly IJSRuntime _jSRuntime;
        private readonly SemaphoreSlim _semaphore;

        protected abstract string JsSIPScriptFile { get; }

        private IJSObjectReference? _context;
        protected async ValueTask<IJSObjectReference> JSContext()
        {
            if (_context != null)
                return _context;

            await _semaphore.WaitAsync();
            try
            {
                if (_context == null)
                {
                    _context = await _jSRuntime.InvokeAsync<IJSObjectReference>("import", JsSIPScriptFile);
                    await OnReady();
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return _context;
        }

        protected abstract ValueTask OnReady();

        public async ValueTask InvokeVoidAsync(string identifier, params object?[]? args)
            => await (await JSContext()).InvokeVoidAsync(identifier, args);

        public async ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken, params object?[]? args)
            => await (await JSContext()).InvokeVoidAsync(identifier, cancellationToken, args);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, params object?[]? args)
            => await(await JSContext()).InvokeAsync<TValue>(identifier, args);
        
        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, params object?[]? args)
            => await(await JSContext()).InvokeAsync<TValue>(identifier, cancellationToken, args);

        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
                _context = null;
            }
        }
    }
}
