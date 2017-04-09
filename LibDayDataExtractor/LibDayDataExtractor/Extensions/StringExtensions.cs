using System.Globalization;

namespace LibDayDataExtractor.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string self, string value, CompareOptions options)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(self, value, options) >= 0;
        }
    }
}
