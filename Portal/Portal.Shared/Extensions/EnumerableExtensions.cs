using Portal.Shared.Interfaces;

namespace Portal.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static string ToCommaString<TValue>(this IEnumerable<TValue> values)
        {
            var result = string.Empty;

            foreach (var value in values.ToList())
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += ",";
                }

                result += value;
            }

            return result;
        }
        public static IEnumerable<IIdName> GetNames(this IEnumerable<IIdName> lookup, IEnumerable<int>? ids)
        {
            var result = new List<IIdName>();

            if (ids == null)
            {
                return result;
            }

            foreach (var id in ids)
            {
                var item = lookup.SingleOrDefault(x => x.Id == id);
                if (item == null)
                    continue;
                result.Add(item);
            }
            return result;
        }

      
    }
}
