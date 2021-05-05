namespace DevFromDownUnder.SQLBrowser.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Just a nicer IsNullOrWhiteSpace, 9/10 whitespace counts as "no value"
        /// </summary>
        public static bool HasNoValue(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool HasValue(this string value)
        {
            return !HasNoValue(value);
        }
    }
}