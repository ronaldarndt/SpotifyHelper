using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyHelper.Core.Extensions;

public static class EnumerableExtensions
{
    public static async Task<bool> Any<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate)
    {
        await foreach (var item in source)
        {
            if (predicate(item))
            {
                return true;
            }
        }

        return false;
    }

}
