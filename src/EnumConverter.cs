using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class EnumConverter<T> : JsonConverter<T> where T : System.Enum
    {
        public override T? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions __)
            => EnumExtensions.GetValueFromDescription<T>(type, reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions _)
            => writer.WriteStringValue(value.GetDescription());        
    }
}
