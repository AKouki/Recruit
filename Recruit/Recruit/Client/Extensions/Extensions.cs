using System.Security.Claims;

namespace Recruit.Client.Extensions
{
    public static class Extensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? data)
        {
            return data ?? Enumerable.Empty<T>();
        }
    }
}
