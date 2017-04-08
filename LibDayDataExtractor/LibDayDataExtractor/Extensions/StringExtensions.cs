using System.Globalization;

namespace LibDayDataExtractor.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string str, string value, CompareOptions options)
        {
            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(str, value, options) >= 0;
        }
    }
}
