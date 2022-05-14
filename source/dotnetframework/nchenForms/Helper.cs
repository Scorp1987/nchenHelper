using System.IO;

namespace System.Windows.Forms
{
    public static class Helper
    {
        public static bool ShowError(string message, string title = "User Input Error", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(message, title, buttons, icon);
            return false;
        }


        internal static string GetInitDir(string value)
        {
            if (string.IsNullOrEmpty(value)) return Directory.GetCurrentDirectory();
            var toReturn = value.ToPath();
            return Path.GetDirectoryName(toReturn);
        }

        internal static string GetFileName(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return Path.GetFileName(value);
        }
    }
}
