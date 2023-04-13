using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public static class EnumExtensions
    {

        public static T? GetValueFromDescription<T>(Type type, string description) where T : Enum
        {
            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T?)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T?)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
        }

        public static T? GetValueFromDescription<T>(string description) where T : Enum
            => GetValueFromDescription<T>(typeof(T), description);  

        public static string GetDescription(this Enum value)
        {
            DescriptionAttribute? attribute = null;
            var field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                            as DescriptionAttribute;
            }
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
