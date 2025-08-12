using System;

namespace ROTools.Utils
{
    public static class StringExtensions
    {
        public static bool ContainsText(this string self, string textToCompare)
        {
            return self.IndexOf(textToCompare, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
