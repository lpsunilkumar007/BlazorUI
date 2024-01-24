using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Portal.Shared.Extensions
{
    public static class ObjectExtensions
    {
        public static T? GetPropValue<T>(this object? obj, string propName)
        {
            if (obj == null)
                return default;
            var field = obj.GetType().GetProperty(propName);
            if (field == null)
                return default;

            var value = field.GetValue(obj, null);
            if (value == null)
                return default;

            return (T)value;
        }

        public static T? GetCopy<T>(this T item)
            where T : new()
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(item));
        }

        public static T? DeepClone<T>(this T item)
            where T : class, new()
        {
            using var ms = new MemoryStream();
            var serializer = new XmlSerializer(item.GetType());
            serializer.Serialize(ms, item);
            ms.Seek(0, SeekOrigin.Begin);

            return serializer.Deserialize(ms) as T;
        }


        public static string Serialize<T>(this T item)
        {
            return JsonSerializer.Serialize(item);
        }

        public static bool IsEqual<T>(this T item, T other)
        {
            return item.Serialize() == other.Serialize();
        }

        public static string? ToJsonBase64String(this object? obj)
        {
            return obj != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj))) : null;
        }

        public static string ToStringFromBase64(this string base64EncodedData)
        {
            return JsonSerializer.Deserialize<string>(Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData)));
        }
    }
}
