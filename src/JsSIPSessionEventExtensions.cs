using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public static class JsSIPSessionEventExtensions
    {
        public static string? ToJsonString(this JsSIPSessionEvent source)
        {
            try { return JsonSerializer.Serialize(source); } catch { return null; }
        }
    }
}
