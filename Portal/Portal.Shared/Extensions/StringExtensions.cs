using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace Portal.Shared.Extensions
{
    public static partial class StringExtensions
    {
        public static string CommaString(this ICollection<string>? items, string? noResultText = null)
        {
            if (items == null)
            {
                return !string.IsNullOrEmpty(noResultText) ? noResultText : "";
            }

            var result = "";
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ", ";
                }

                result += item;
            }

            return result;
        }
        public static string AppendId(this string item, int id)
        {
            return item + (string.IsNullOrEmpty(item) ? id.ToString() : $",{id}");
        }

        public static string AppendValue(this string item, string value)
        {
            return item + (string.IsNullOrEmpty(item) ? value : $", {value}");
        }

        public static string AddQueryStringParameter<TParamType>(this string? query, string paramName, TParamType paramValue)
        {
            if (string.IsNullOrEmpty(paramName))
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            var result = string.IsNullOrEmpty(query) ? "" : query;

            if (!result.Contains('?'))
            {
                result += "?";
            }

            if (result.Last() != '?')
            {
                result += "&";
            }

            var paramValueString = Convert.ToString(paramValue);
            if (typeof(TParamType) == typeof(string))
            {
                paramValueString = HttpUtility.UrlEncode(paramValueString);
            }

            result += $"{paramName}={paramValueString}";

            return result;
        }

        public static string AddQueryStringParameter(this string query, string paramName, int? paramValue)
        {
            return paramValue.HasValue ? query.AddQueryStringParameter(paramName, paramValue.Value) : query;
        }

        public static string AddQueryStringParameter(this string query, string paramName, string? paramValue)
        {
            return !string.IsNullOrEmpty(paramValue) ? query.AddQueryStringParameter<string>(paramName, paramValue) : query;
        }

        public static string AddQueryStringParameter(this string query, string paramName, DateTimeOffset? paramValue)
        {
            return paramValue.HasValue ? query.AddQueryStringParameter(paramName, paramValue.Value) : query;
        }

        public static string AddQueryStringParameter(this string query, string paramName, DateTimeOffset paramValue)
        {
            return query.AddQueryStringParameter<string>(paramName, paramValue.ToString("O"));
        }

        //public static string FromCamelCase(this string str)
        //{
        //    return CamelCaseRegex1().Replace(CamelCaseRegex2().Replace(str, "$1 $2"), "$1 $2");
        //}

        public static bool In(this string? @this, params string[] values)
        {
            if (string.IsNullOrEmpty(@this))
                return false;

            foreach (var value in values)
            {
                if (@this.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        public static bool In(this string? @this, IList<string> values)
        {
            return @this.In(values.ToArray());
        }

        //[GeneratedRegex("(\\p{Ll})(\\P{Ll})")]
       // private static partial Regex CamelCaseRegex1();
        //[GeneratedRegex("(\\P{Ll})(\\P{Ll}\\p{Ll})")]
        //private static partial Regex CamelCaseRegex2();

        public static T? FromJsonBase64String<T>(this string? base64EncodedData)
        {
            if (string.IsNullOrWhiteSpace(base64EncodedData))
            {
                return default(T);
            }

            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(base64EncodedBytes));
        }
    }
}
