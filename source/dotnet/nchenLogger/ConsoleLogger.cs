using System.Threading.Tasks;

namespace System.IO
{
    public class ConsoleLogger : IDisposable
    {
        public const TimeUnit DEFAULT_UNIT = TimeUnit.Second;

        private string _appLogFilePath;
        private string _expLogFilePath;
        private StreamWriter _applog;
        private StreamWriter _expLog;


        #region OpenLog
        private void CreateDirectoryIfNeeded(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
        private void OpenLog(string filePath, ref string logFilePath, ref StreamWriter writer)
        {
            try
            {
                if (writer != null) CloseLog(ref logFilePath, ref writer);
                CreateDirectoryIfNeeded(filePath);
                var stream = File.Open(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                logFilePath = filePath;
                writer = new StreamWriter(stream);
            }
            catch { }
        }
        public void OpenAppLog(string filePath) => OpenLog(filePath, ref _appLogFilePath, ref _applog);
        public void OpenExpLog(string filePath) => OpenLog(filePath, ref _expLogFilePath, ref _expLog);
        #endregion


        #region CloseLog and Dispose
        private void CloseLog(ref string logFilePath, ref StreamWriter writer)
        {
            try
            {
                writer?.Flush();
                var toDelete = (writer?.BaseStream?.Length < 1);
                writer?.Dispose();
                if (toDelete) File.Delete(logFilePath);
                logFilePath = null;
                writer = null;
            }
            catch { }
        }
        public void CloseAppLog() => CloseLog(ref _appLogFilePath, ref _applog);
        public void CloseExpLog() => CloseLog(ref _expLogFilePath, ref _expLog);
        public void Dispose()
        {
            CloseAppLog();
            CloseExpLog();
        }
        #endregion


        #region Write Additional Message
        public void WriteLog(string message)
        {
            try
            {
                var now = DateTime.Now;
                var str = $"{now:yyyy-MM-dd HH:mm:ss} | {message}";
                Console.WriteLine(str);
                _applog?.WriteLine(str);
            }
            catch { }
        }
        public async Task WriteLogAsync(string message)
        {
            try
            {
                var now = DateTime.Now;
                var str = $"{now:yyyy-MM-dd HH:mm:ss} | {message}";
                Console.WriteLine(str);
                await _applog?.WriteLineAsync(str);
            }
            catch { }
        }
        public void WriteException(string taskName, Exception ex)
        {
            try
            {
                _expLog?.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {ex.Message} occured at {taskName}");
                _expLog?.WriteLine(ex.StackTrace);
                _expLog?.WriteLine();
            }
            catch { }
        }
        public async Task WriteExceptionAsync(string taskName, Exception ex)
        {
            try
            {
                await _expLog?.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {ex.Message} occured at {taskName}");
                await _expLog?.WriteLineAsync(ex.StackTrace);
                await _expLog?.WriteLineAsync();
            }
            catch { }
        }
        #endregion


        #region RunTask
        private DateTime WriteLogStart(string taskName)
        {
            var now = DateTime.Now;
            try
            {
                var str = $"{now:yyyy-MM-dd HH:mm:ss} | {taskName}...";
                Console.Write(str);
                _applog?.Write(str);
            }
            catch { }
            return now;
        }
        private void WriteLogComplete(string message)
        {
            try
            {
                Console.WriteLine(message);
                _applog?.WriteLine(message);
            }
            catch { }
        }
        private string GetString(TimeSpan timeTaken, TimeUnit unit = DEFAULT_UNIT)
        {
            return unit switch
            {
                TimeUnit.MilliSecond => $"{timeTaken.TotalMilliseconds:0.000} msec",
                TimeUnit.Second => $"{timeTaken.TotalSeconds:0.000} sec",
                TimeUnit.Minute => $"{timeTaken.TotalMinutes:0.000} min",
                TimeUnit.Hour => $"{timeTaken.TotalHours:0.000} hr",
                TimeUnit.Day => $"{timeTaken.TotalDays:0.000} day",
                _ => throw new NotImplementedException(),
            };
        }

        public bool RunTask(string taskName, Action function, TimeUnit unit = DEFAULT_UNIT)
        {
            var startDate = WriteLogStart(taskName);
            try
            {
                function();
                var timeTaken = DateTime.Now - startDate;
                WriteLogComplete($"Successful. TimeTaken: {GetString(timeTaken, unit)}");
                return true;
            }
            catch (Exception ex)
            {
                WriteException(taskName, ex);
                var timeTaken = DateTime.Now - startDate;
                WriteLogComplete($"Error. TimeTaken: {GetString(timeTaken, unit)}");
                return false;
            }
        }
        public TResult RunTask<TResult>(string taskName, Func<TResult> function, Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        {
            var startDate = WriteLogStart(taskName);
            try
            {
                var result = function();
                var additionalResultStr = getAdditionalResult(result);
                additionalResultStr = string.IsNullOrEmpty(additionalResultStr) ? "" : $" {additionalResultStr}.";
                var status = getStatus(result);
                var timeTaken = DateTime.Now - startDate;
                WriteLogComplete($"{status}.{additionalResultStr} TimeTaken: {GetString(timeTaken, unit)}");
                return result;
            }
            catch (Exception ex)
            {
                WriteException(taskName, ex);
                var timeTaken = DateTime.Now - startDate;
                WriteLogComplete($"Error. TimeTaken: {GetString(timeTaken, unit)}");
                return default;
            }
        }
        public TResult RunTask<TResult>(string taskName, Func<TResult> function, Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT)
            => RunTask(taskName, function, getAdditionalResult, result => "Successful", unit);
        public TResult RunTask<TResult>(string taskName, Func<TResult> function, TimeUnit unit = DEFAULT_UNIT)
            => RunTask(taskName, function, result => "", unit);

        public async Task<bool> RunTaskAsync(string taskName, Func<Task> function, TimeUnit unit = DEFAULT_UNIT)
        {
            var startDate = WriteLogStart(taskName);
            try
            {
                await function();
                var timeTaken = DateTime.Now - startDate;
                WriteLogComplete($"Successful. TimeTaken: {GetString(timeTaken, unit)}");
                return true;
            }
            catch (Exception ex)
            {
                WriteException(taskName, ex);
                var timeTaken = DateTime.Now - startDate;
                WriteLogComplete($"Error. TimeTaken: {GetString(timeTaken, unit)}");
                return false;
            }
        }
        public async Task<TResult> RunTaskAsync<TResult>(string taskName, Func<Task<TResult>> function, Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        {
            var startDate = WriteLogStart(taskName);
            try
            {
                var result = await function();
                var additionalResultStr = getAdditionalResult(result);
                additionalResultStr = string.IsNullOrEmpty(additionalResultStr) ? "" : $" {additionalResultStr}.";
                var status = getStatus(result);
                var timeTaken = DateTime.Now - startDate;
                WriteLogComplete($"{status}.{additionalResultStr} TimeTaken: {GetString(timeTaken, unit)}");
                return result;
            }
            catch (Exception ex)
            {
                WriteException(taskName, ex);
                var timeTaken = DateTime.Now - startDate;
                WriteLogComplete($"Error. TimeTaken: {GetString(timeTaken, unit)}");
                return default;
            }
        }
        public Task<TResult> RunTaskAsync<TResult>(string taskName, Func<Task<TResult>> function, Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT)
            => RunTaskAsync(taskName, function, getAdditionalResult, result => "Successful", unit);
        public Task<TResult> RunTaskAsync<TResult>(string taskName, Func<Task<TResult>> function, TimeUnit unit = DEFAULT_UNIT)
            => RunTaskAsync(taskName, function, result => "", unit);
        #endregion

    }
}
