using System.Security.Claims;

namespace Recruit.Client.Extensions
{
    public static class Extensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? data)
        {
            return data ?? Enumerable.Empty<T>();
        }

        public static void Replace<T>(this List<T>? data, T oldItem, T newItem)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            int index = data.IndexOf(oldItem);
            if (index != -1)
                data[index] = newItem;
        }
    }
}
