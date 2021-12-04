using System;
using System.Runtime.CompilerServices;

namespace SpotifyHelper.Core.Extensions;

public static class EnumExtensions
{
    public static T SetFlagIf<T>(this T source, T flag, bool condition) where T : Enum
    {
        if (condition)
        {
            var result = ToInt(source) | ToInt(flag);

            return (T)Enum.ToObject(typeof(T), result);
        }

        return source;

        static int ToInt(T source) => Unsafe.As<T, int>(ref source); 
    }

}

