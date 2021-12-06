using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP.Extensions
{
    public static class JsRuntimeExtensions
    {
        public static async Task<JsObjectReferenceDynamic> Import<T>(this IJSRuntime jsRuntime,
            string pathFromWwwRoot)
        {
            var libraryName = typeof(T).Assembly.GetName().Name;
            var module = await jsRuntime.InvokeAsync<JSObjectReference>(
                "import",
                Path.Combine($"./_content/{libraryName}/", pathFromWwwRoot)
            );
            return new JsObjectReferenceDynamic(module);
        }

        public static async Task<JsObjectReferenceDynamic> Import(this IJSRuntime jsRuntime,
            string libraryName,
            string pathFromWwwRoot)
        {
            var module = await jsRuntime.InvokeAsync<JSObjectReference>(
                "import",
                Path.Combine($"./_content/{libraryName}/", pathFromWwwRoot)
            );
            return new JsObjectReferenceDynamic(module);
        }
    }
}
