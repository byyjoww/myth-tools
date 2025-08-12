using System;

namespace ROTools.Utils
{
    public static class EnumExtensions
    {
        public static T ParseEnumIgnoringCaseOrDefault<T>(string value) where T : struct
        {
            return Enum.TryParse(value: value, ignoreCase: true, out T dt)
                ? dt
                : default(T);
        }
    }
}
