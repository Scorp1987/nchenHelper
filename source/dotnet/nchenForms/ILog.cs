namespace System.Windows.Forms
{
    public interface ILog
    {
        string ObjectName { get; }

        void LogUI(string message);

        void LogError(string message, Exception ex);
    }
}
