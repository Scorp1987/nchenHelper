using System.IO;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert from shortform Path to full file/folder Path
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPath(this string str)
        {
            if (str == null) return null;
            var regex = new Regex("([%][^%]+[%])");
            var path = regex.Replace(str, m =>
            {
                var value = m.Value.Substring(1, m.Value.Length - 2);
                var specialFolder = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), value, true);
                return Environment.GetFolderPath(specialFolder);
            });
            if (!path.Contains(":")) path = Path.Combine(Directory.GetCurrentDirectory(), path);
            return path;
        }

        /// <summary>
        /// Convert from full file/folder path to shortform path
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToShortFormPath(this string str)
        {
            if (str == null) return null;
            var toReturn = str;
            toReturn = toReturn.Replace(AppDomain.CurrentDomain.BaseDirectory, "");
            foreach (Environment.SpecialFolder sf in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                var spfp = Environment.GetFolderPath(sf);
                if (!string.IsNullOrEmpty(spfp))
                    toReturn = toReturn.Replace(Environment.GetFolderPath(sf), $"%{sf}%");
            }
            return toReturn;
        }
    }
}
