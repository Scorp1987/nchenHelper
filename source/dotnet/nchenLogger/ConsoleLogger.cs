using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.IO
{
    public class ConsoleLogger : IDisposable
    {
        public const TimeUnit DEFAULT_UNIT = TimeUnit.Second;

        public bool AllowConsoleLog { get; set; } = true;
        public bool AllowAppLog => _applog?.BaseStream?.CanWrite == true;
        public bool AllowExpLog => _expLog?.BaseStream?.CanWrite == true;


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
                writer = new StreamWriter(stream) { AutoFlush = true };
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
                if (AllowConsoleLog) Console.WriteLine(str);
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
                if (AllowConsoleLog) Console.WriteLine(str);
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
        private DateTime WriteLogStart(string taskName)
        {
            var now = DateTime.Now;
            try
            {
                var str = $"{now:yyyy-MM-dd HH:mm:ss} | {taskName}...";
                if (AllowConsoleLog) Console.Write(str);
                _applog?.Write(str);
            }
            catch { }
            return now;
        }
        private void WriteLogComplete(string message)
        {
            try
            {
                if (AllowConsoleLog) Console.WriteLine(message);
                _applog?.WriteLine(message);
            }
            catch { }
        }
        private void WriteLogComplete(string result, string additionalResult, DateTime start, TimeUnit unit)
        {
            var timeTaken = DateTime.Now - start;
            if (!string.IsNullOrEmpty(additionalResult))
                additionalResult = $" {additionalResult}.";
            WriteLogComplete($"{result}.{additionalResult} TimeTaken: {GetString(timeTaken, unit)}");
        }
        private void WriteLogComplete(string taskName, string result, string additionalResult, DateTime start, TimeUnit unit)
        {
            var timeTaken = DateTime.Now - start;
            if (!string.IsNullOrEmpty(additionalResult))
                additionalResult = $" {additionalResult}.";
            WriteLog($"{taskName} {result}.{additionalResult} TimeTaken: {GetString(timeTaken, unit)}");
        }

        private async Task<DateTime> WriteLogStartAsync(string taskName)
        {
            var now = DateTime.Now;
            try
            {
                var str = $"{now:yyyy-MM-dd HH:mm:ss} | {taskName}...";
                if (AllowConsoleLog) Console.Write(str);
                await _applog?.WriteAsync(str);
            }
            catch { }
            return now;
        }
        private async Task WriteLogCompleteAsync(string message)
        {
            try
            {
                if (AllowConsoleLog) Console.WriteLine(message);
                await _applog?.WriteLineAsync(message);
            }
            catch { }
        }
        private async Task WriteLogCompleteAsync(string result, string additionalResult, DateTime start, TimeUnit unit)
        {
            var timeTaken = DateTime.Now - start;
            if (!string.IsNullOrEmpty(additionalResult))
                additionalResult = $" {additionalResult}.";
            await WriteLogCompleteAsync($"{result}.{additionalResult} TimeTaken: {GetString(timeTaken, unit)}");
        }
        private async Task WriteLogCompleteAsync(string taskName, string result, string additionalResult, DateTime start, TimeUnit unit)
        {
            var timeTaken = DateTime.Now - start;
            if (!string.IsNullOrEmpty(additionalResult))
                additionalResult = $" {additionalResult}.";
            await WriteLogAsync($"{taskName} {result}.{additionalResult} TimeTaken: {GetString(timeTaken, unit)}");
        }


        private RunResult RunComplete(RunResult result, string resultStr, string additionalResult, DateTime start, TimeUnit unit)
        {
            WriteLogComplete(resultStr, additionalResult, start, unit);
            return result;
        }
        private RunResult RunComplete(RunResult result, DateTime start, TimeUnit unit) =>
            RunComplete(result, result.ToString(), null, start, unit);
        private RunResult RunComplete(RunResult result, DateTime start, TimeUnit unit, string taskName, Exception ex)
        {
            WriteException(taskName, ex);
            return RunComplete(result, start, unit);
        }
        private RunResult RunComplete(string taskName, RunResult result, string resultStr, string additionalResult, DateTime start, TimeUnit unit)
        {
            WriteLogComplete(taskName, resultStr, additionalResult, start, unit);
            return result;
        }
        private RunResult RunComplete(string taskName, RunResult result, DateTime start, TimeUnit unit) =>
            RunComplete(taskName, result, result.ToString(), null, start, unit);
        private RunResult RunComplete(string taskName, RunResult result, DateTime start, TimeUnit unit, Exception ex)
        {
            WriteException(taskName, ex);
            return RunComplete(taskName, result, start, unit);
        }

        private async Task<RunResult> RunCompleteAsync(RunResult result, string resultStr, string additionalResult, DateTime start, TimeUnit unit)
        {
            await WriteLogCompleteAsync(resultStr, additionalResult, start, unit);
            return result;
        }
        private Task<RunResult> RunCompleteAsync(RunResult result, DateTime start, TimeUnit unit) =>
            RunCompleteAsync(result, result.ToString(), null, start, unit);
        private async Task<RunResult> RunCompleteAsync(RunResult result, DateTime start, TimeUnit unit, string taskName, Exception ex)
        {
            await WriteExceptionAsync(taskName, ex);
            return await RunCompleteAsync(result, start, unit);
        }
        private async Task<RunResult> RunCompleteAsync(string taskName, RunResult result, string resultStr, string additionalResult, DateTime start, TimeUnit unit)
        {
            await WriteLogCompleteAsync(taskName, resultStr, additionalResult, start, unit);
            return result;
        }
        private Task<RunResult> RunCompleteAsync(string taskName, RunResult result, DateTime start, TimeUnit unit) =>
            RunCompleteAsync(taskName, result, result.ToString(), null, start, unit);
        private async Task<RunResult> RunCompleteAsync(string taskName, RunResult result, DateTime start, TimeUnit unit, Exception ex)
        {
            await WriteExceptionAsync(taskName, ex);
            return await RunCompleteAsync(taskName, result, start, unit);
        }


        public RunResult RunFastTask(string taskName, Action action, TimeUnit unit = DEFAULT_UNIT)
        {
            var start = WriteLogStart(taskName);
            try
            {
                action();
                return RunComplete(RunResult.Successful, start, unit);
            }
            catch (OperationCanceledException ex)
            {
                return RunComplete(RunResult.Cancel, start, unit, taskName, ex);
            }
            catch (TimeoutException ex)
            {
                return RunComplete(RunResult.Timeout, start, unit, taskName, ex);
            }
            catch (Exception ex)
            {
                return RunComplete(RunResult.Error, start, unit, taskName, ex);
            }
        }
        public RunResult RunFastTask<TResult>(string taskName, Func<TResult> function,
            Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, out TResult output, TimeUnit unit = DEFAULT_UNIT)
        {
            var start = WriteLogStart(taskName);
            output = default;
            try
            {
                output = function();
                var additionalResultStr = getAdditionalResult(output);
                var status = getStatus(output);
                return RunComplete(RunResult.Successful, status, additionalResultStr, start, unit);
            }
            catch (OperationCanceledException ex)
            {
                return RunComplete(RunResult.Cancel, start, unit, taskName, ex);
            }
            catch (TimeoutException ex)
            {
                return RunComplete(RunResult.Timeout, start, unit, taskName, ex);
            }
            catch (Exception ex)
            {
                return RunComplete(RunResult.Error, start, unit, taskName, ex);
            }
        }
        public RunResult RunFastTask<TResult>(string taskName, Func<TResult> function,
            Func<TResult, string> getAdditionalResult, out TResult output, TimeUnit unit = DEFAULT_UNIT) =>
            RunFastTask(taskName, function, getAdditionalResult, r => RunResult.Successful.ToString(), out output, unit);
        public RunResult RunFastTask<TResult>(string taskName, Func<TResult> function, out TResult output, TimeUnit unit = DEFAULT_UNIT) =>
            RunFastTask(taskName, function, r => null, out output, unit);

        public async Task<RunResult> RunFastTaskAsync(string taskName, Func<Task> action, TimeUnit unit = DEFAULT_UNIT)
        {
            var start = await WriteLogStartAsync(taskName);
            try
            {
                await action();
                return await RunCompleteAsync(RunResult.Successful, start, unit);
            }
            catch (OperationCanceledException ex)
            {
                return await RunCompleteAsync(RunResult.Cancel, start, unit, taskName, ex);
            }
            catch (TimeoutException ex)
            {
                return await RunCompleteAsync(RunResult.Timeout, start, unit, taskName, ex);
            }
            catch (Exception ex)
            {
                return await RunCompleteAsync(RunResult.Error, start, unit, taskName, ex);
            }
        }
        public async Task<KeyValuePair<RunResult, TResult>> RunFastTaskAsync<TResult>(string taskName, Func<Task<TResult>> function,
            Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        {
            var start = WriteLogStart(taskName);
            try
            {
                var output = await function();
                var additionalResultStr = getAdditionalResult(output);
                var status = getStatus(output);
                var result = await RunCompleteAsync(RunResult.Successful, status, additionalResultStr, start, unit);
                return new KeyValuePair<RunResult, TResult>(result, output);
            }
            catch (OperationCanceledException ex)
            {
                var result = await RunCompleteAsync(RunResult.Cancel, start, unit, taskName, ex);
                return new KeyValuePair<RunResult, TResult>(result, default);
            }
            catch (TimeoutException ex)
            {
                var result = await RunCompleteAsync(RunResult.Timeout, start, unit, taskName, ex);
                return new KeyValuePair<RunResult, TResult>(result, default);
            }
            catch (Exception ex)
            {
                var result = await RunCompleteAsync(RunResult.Error, start, unit, taskName, ex);
                return new KeyValuePair<RunResult, TResult>(result, default);
            }
        }
        public Task<KeyValuePair<RunResult, TResult>> RunFastTaskAsync<TResult>(string taskName, Func<Task<TResult>> function,
            Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT) =>
            RunFastTaskAsync(taskName, function, getAdditionalResult, r => RunResult.Successful.ToString(), unit);
        public Task<KeyValuePair<RunResult, TResult>> RunFastTaskAsync<TResult>(string taskName, Func<Task<TResult>> function, TimeUnit unit = DEFAULT_UNIT) =>
            RunFastTaskAsync(taskName, function, r => null, unit);


        public RunResult RunSlowTask(string taskName, Action action, TimeUnit unit = DEFAULT_UNIT)
        {
            var start = DateTime.Now;
            WriteLog($"{taskName} Started.");
            try
            {
                action();
                return RunComplete(taskName, RunResult.Successful, start, unit);
            }
            catch (OperationCanceledException ex)
            {
                return RunComplete(taskName, RunResult.Cancel, start, unit, ex);
            }
            catch (TimeoutException ex)
            {
                return RunComplete(taskName, RunResult.Timeout, start, unit, ex);
            }
            catch (Exception ex)
            {
                return RunComplete(taskName, RunResult.Error, start, unit, ex);
            }
        }
        public RunResult RunSlowTask<TResult>(string taskName, Func<TResult> function,
            Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, out TResult output, TimeUnit unit = DEFAULT_UNIT)
        {
            var start = DateTime.Now;
            WriteLog($"{taskName} Started.");
            output = default;
            try
            {
                output = function();
                var additionalResultStr = getAdditionalResult(output);
                var status = getStatus(output);
                return RunComplete(taskName, RunResult.Successful, status, additionalResultStr, start, unit);
            }
            catch (OperationCanceledException ex)
            {
                return RunComplete(taskName, RunResult.Cancel, start, unit, ex);
            }
            catch (TimeoutException ex)
            {
                return RunComplete(taskName, RunResult.Timeout, start, unit, ex);
            }
            catch (Exception ex)
            {
                return RunComplete(taskName, RunResult.Error, start, unit, ex);
            }
        }
        public RunResult RunSlowTask<TResult>(string taskName, Func<TResult> function,
            Func<TResult, string> getAdditionalResult, out TResult output, TimeUnit unit = DEFAULT_UNIT) =>
            RunSlowTask(taskName, function, getAdditionalResult, r => RunResult.Successful.ToString(), out output, unit);
        public RunResult RunSlowTask<TResult>(string taskName, Func<TResult> function, out TResult output, TimeUnit unit = DEFAULT_UNIT) =>
            RunSlowTask(taskName, function, r => null, out output, unit);

        public async Task<RunResult> RunSlowTaskAsync(string taskName, Func<Task> action, TimeUnit unit = DEFAULT_UNIT)
        {
            var start = DateTime.Now;
            await WriteLogAsync($"{taskName} Started.");
            try
            {
                await action();
                return await RunCompleteAsync(taskName, RunResult.Successful, start, unit);
            }
            catch (OperationCanceledException ex)
            {
                return await RunCompleteAsync(taskName, RunResult.Cancel, start, unit, ex);
            }
            catch (TimeoutException ex)
            {
                return await RunCompleteAsync(taskName, RunResult.Timeout, start, unit, ex);
            }
            catch (Exception ex)
            {
                return await RunCompleteAsync(taskName, RunResult.Error, start, unit, ex);
            }
        }
        public async Task<KeyValuePair<RunResult, TResult>> RunSlowTaskAsync<TResult>(string taskName, Func<Task<TResult>> function,
            Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        {
            var start = DateTime.Now;
            await WriteLogAsync($"{taskName} Started.");
            try
            {
                var output = await function();
                var additionalResultStr = getAdditionalResult(output);
                var status = getStatus(output);
                var result = await RunCompleteAsync(taskName, RunResult.Successful, status, additionalResultStr, start, unit);
                return new KeyValuePair<RunResult, TResult>(result, output);
            }
            catch (OperationCanceledException ex)
            {
                var result = await RunCompleteAsync(taskName, RunResult.Cancel, start, unit, ex);
                return new KeyValuePair<RunResult, TResult>(result, default);
            }
            catch (TimeoutException ex)
            {
                var result = await RunCompleteAsync(taskName, RunResult.Timeout, start, unit, ex);
                return new KeyValuePair<RunResult, TResult>(result, default);
            }
            catch (Exception ex)
            {
                var result = await RunCompleteAsync(taskName, RunResult.Error, start, unit, ex);
                return new KeyValuePair<RunResult, TResult>(result, default);
            }
        }
        public Task<KeyValuePair<RunResult, TResult>> RunSlowTaskAsync<TResult>(string taskName, Func<Task<TResult>> function,
            Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT) =>
            RunSlowTaskAsync(taskName, function, getAdditionalResult, r => RunResult.Successful.ToString(), unit);
        public Task<KeyValuePair<RunResult, TResult>> RunSlowTaskAsync<TResult>(string taskName, Func<Task<TResult>> function, TimeUnit unit = DEFAULT_UNIT) =>
            RunSlowTaskAsync(taskName, function, r => null, unit);






        //public void RunFastTaskThrowException(string taskName, Action action, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var start = WriteLogStart(taskName);
        //    try
        //    {
        //        action();
        //        WriteLogComplete("Successful", null, start, unit);
        //    }
        //    catch(OperationCanceledException ex) { WriteLogComplete("Cancel", null, start, unit); throw ex; }
        //    catch(TimeoutException ex) { WriteLogComplete("Timeout", null, start, unit); throw ex; }
        //    catch(Exception ex) { WriteLogComplete("Error", null, start, unit); throw ex; }
        //}
        //public TResult RunFastTaskThrowException<TResult>(string taskName, Func<TResult> function, Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var start = WriteLogStart(taskName);
        //    try
        //    {
        //        var result = function();
        //        var additionalResultStr = getAdditionalResult(result);
        //        var status = getStatus(result);
        //        WriteLogComplete(status, additionalResultStr, start, unit);
        //        return result;
        //    }
        //    catch (OperationCanceledException ex) { WriteLogComplete("Cancel", null, start, unit); throw ex; }
        //    catch (TimeoutException ex) { WriteLogComplete("Timeout", null, start, unit); throw ex; }
        //    catch (Exception ex) { WriteLogComplete("Error", null, start, unit); throw ex; }
        //}
        //public TResult RunFastTaskThrowException<TResult>(string taskName, Func<TResult> function, Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT) =>
        //    RunFastTaskThrowException(taskName, function, getAdditionalResult, result => "Successful", unit);
        //public TResult RunFastTaskThrowException<TResult>(string taskName, Func<TResult> function, TimeUnit unit = DEFAULT_UNIT) =>
        //    RunFastTaskThrowException(taskName, function, result => null, unit);

        //public async Task RunFastTaskThrowExceptionAsync(string taskName, Func<Task> action, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var start = await WriteLogStartAsync(taskName);
        //    try
        //    {
        //        await action();
        //        await WriteLogCompleteAsync("Successful", null, start, unit);
        //    }
        //    catch (OperationCanceledException ex) { await WriteLogCompleteAsync("Cancel", null, start, unit); throw ex; }
        //    catch (TimeoutException ex) { await WriteLogCompleteAsync("Timeout", null, start, unit); throw ex; }
        //    catch (Exception ex) { await WriteLogCompleteAsync("Error", null, start, unit); throw ex; }
        //}
        //public async Task<TResult> RunFastTaskThrowExceptionAsync<TResult>(string taskName, Func<Task<TResult>> function, Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var start = await WriteLogStartAsync(taskName);
        //    try
        //    {
        //        var result = await function();
        //        var additionalResultStr = getAdditionalResult(result);
        //        var status = getStatus(result);
        //        await WriteLogCompleteAsync(status, additionalResultStr, start, unit);
        //        return result;
        //    }
        //    catch (OperationCanceledException ex) { await WriteLogCompleteAsync("Cancel", null, start, unit); throw ex; }
        //    catch (TimeoutException ex) { await WriteLogCompleteAsync("Timeout", null, start, unit); throw ex; }
        //    catch (Exception ex) { await WriteLogCompleteAsync("Error", null, start, unit); throw ex; }
        //}
        //public Task<TResult> RunFastTaskThrowExceptionAsync<TResult>(string taskName, Func<Task<TResult>> function, Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT) =>
        //    RunFastTaskThrowExceptionAsync(taskName, function, getAdditionalResult, result => "Successful", unit);
        //public Task<TResult> RunFastTaskThrowExceptionAsync<TResult>(string taskName, Func<Task<TResult>> function, TimeUnit unit = DEFAULT_UNIT) =>
        //    RunFastTaskThrowExceptionAsync(taskName, function, result => null, unit);

        //public void RunSlowTaskThrowException(string taskName, Action action, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var start = DateTime.Now;
        //    WriteLog($"{taskName} Started.");
        //    try
        //    {
        //        action();
        //        WriteLogComplete(taskName, "Successful", null, start, unit);
        //    }
        //    catch(OperationCanceledException ex) { WriteLogComplete(taskName, "Cancel", null, start, unit); throw ex; }
        //    catch (TimeoutException ex) { WriteLogComplete(taskName, "Timeout", null, start, unit); throw ex; }
        //    catch (Exception ex) { WriteLogComplete(taskName, "Error", null, start, unit); throw ex; }
        //}
        //public TResult RunSlowTaskThrowException<TResult>(string taskName, Func<TResult> function, Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var start = DateTime.Now;
        //    WriteLog($"{taskName} Started.");
        //    try
        //    {
        //        var result = function();
        //        var additionalResultStr = getAdditionalResult(result);
        //        var status = getStatus(result);
        //        WriteLogComplete(taskName, status, additionalResultStr, start, unit);
        //        return result;
        //    }
        //    catch (OperationCanceledException ex) { WriteLogComplete(taskName, "Cancel", null, start, unit); throw ex; }
        //    catch (TimeoutException ex) { WriteLogComplete(taskName, "Timeout", null, start, unit); throw ex; }
        //    catch (Exception ex) { WriteLogComplete(taskName, "Error", null, start, unit); throw ex; }
        //}
        //public TResult RunSlowTaskThrowException<TResult>(string taskName, Func<TResult> function, Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT) =>
        //    RunSlowTaskThrowException(taskName, function, getAdditionalResult, result => "Successful", unit);
        //public TResult RunSlowTaskThrowException<TResult>(string taskName, Func<TResult> function, TimeUnit unit = DEFAULT_UNIT) =>
        //    RunSlowTaskThrowException(taskName, function, result => null, unit);

        //public async Task RunSlowTaskThrowExceptionAsync(string taskName, Func<Task> action, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var start = DateTime.Now;
        //    await WriteLogAsync($"{taskName} Started.");
        //    try
        //    {
        //        await action();
        //        await WriteLogCompleteAsync(taskName, "Successful", null, start, unit);
        //    }
        //    catch (OperationCanceledException ex) { await WriteLogCompleteAsync(taskName, "Cancel", null, start, unit); throw ex; }
        //    catch (TimeoutException ex) { await WriteLogCompleteAsync(taskName, "Timeout", null, start, unit); throw ex; }
        //    catch (Exception ex) { await WriteLogCompleteAsync(taskName, "Error", null, start, unit); throw ex; }
        //}
        //public async Task<TResult> RunSlowTaskThrowExceptionAsync<TResult>(string taskName, Func<Task<TResult>> function, Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var start = DateTime.Now;
        //    await WriteLogAsync($"{taskName} Started.");
        //    try
        //    {
        //        var result = await function();
        //        var additionalResultStr = getAdditionalResult(result);
        //        var status = getStatus(result);
        //        await WriteLogCompleteAsync(taskName, status, additionalResultStr, start, unit);
        //        return result;
        //    }
        //    catch (OperationCanceledException ex) { await WriteLogCompleteAsync(taskName, "Cancel", null, start, unit); throw ex; }
        //    catch (TimeoutException ex) { await WriteLogCompleteAsync(taskName, "Timeout", null, start, unit); throw ex; }
        //    catch (Exception ex) { await WriteLogCompleteAsync(taskName, "Error", null, start, unit); throw ex; }
        //}
        //public Task<TResult> RunSlowTaskThrowExceptionAsync<TResult>(string taskName, Func<Task<TResult>> function, Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT) =>
        //    RunSlowTaskThrowExceptionAsync(taskName, function, getAdditionalResult, result => "Successful", unit);
        //public Task<TResult> RunSlowTaskThrowExceptionAsync<TResult>(string taskName, Func<Task<TResult>> function, TimeUnit unit = DEFAULT_UNIT) =>
        //    RunSlowTaskThrowException(taskName, function, result => null, unit);

        //public bool RunTask(string taskName, Action function, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var startDate = WriteLogStart(taskName);
        //    try
        //    {
        //        function();
        //        var timeTaken = DateTime.Now - startDate;
        //        WriteLogComplete($"Successful. TimeTaken: {GetString(timeTaken, unit)}");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteException(taskName, ex);
        //        var timeTaken = DateTime.Now - startDate;
        //        WriteLogComplete($"Error. TimeTaken: {GetString(timeTaken, unit)}");
        //        return false;
        //    }
        //}
        //public TResult RunTask<TResult>(string taskName, Func<TResult> function, Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var startDate = WriteLogStart(taskName);
        //    try
        //    {
        //        var result = function();
        //        var additionalResultStr = getAdditionalResult(result);
        //        additionalResultStr = string.IsNullOrEmpty(additionalResultStr) ? "" : $" {additionalResultStr}.";
        //        var status = getStatus(result);
        //        var timeTaken = DateTime.Now - startDate;
        //        WriteLogComplete($"{status}.{additionalResultStr} TimeTaken: {GetString(timeTaken, unit)}");
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteException(taskName, ex);
        //        var timeTaken = DateTime.Now - startDate;
        //        WriteLogComplete($"Error. TimeTaken: {GetString(timeTaken, unit)}");
        //        return default;
        //    }
        //}
        //public TResult RunTask<TResult>(string taskName, Func<TResult> function, Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT)
        //    => RunTask(taskName, function, getAdditionalResult, result => "Successful", unit);
        //public TResult RunTask<TResult>(string taskName, Func<TResult> function, TimeUnit unit = DEFAULT_UNIT)
        //    => RunTask(taskName, function, result => "", unit);

        //public async Task<bool> RunTaskAsync(string taskName, Func<Task> function, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var startDate = WriteLogStart(taskName);
        //    try
        //    {
        //        await function();
        //        var timeTaken = DateTime.Now - startDate;
        //        WriteLogComplete($"Successful. TimeTaken: {GetString(timeTaken, unit)}");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteException(taskName, ex);
        //        var timeTaken = DateTime.Now - startDate;
        //        WriteLogComplete($"Error. TimeTaken: {GetString(timeTaken, unit)}");
        //        return false;
        //    }
        //}
        //public async Task<TResult> RunTaskAsync<TResult>(string taskName, Func<Task<TResult>> function, Func<TResult, string> getAdditionalResult, Func<TResult, string> getStatus, TimeUnit unit = DEFAULT_UNIT)
        //{
        //    var startDate = WriteLogStart(taskName);
        //    try
        //    {
        //        var result = await function();
        //        var additionalResultStr = getAdditionalResult(result);
        //        additionalResultStr = string.IsNullOrEmpty(additionalResultStr) ? "" : $" {additionalResultStr}.";
        //        var status = getStatus(result);
        //        var timeTaken = DateTime.Now - startDate;
        //        WriteLogComplete($"{status}.{additionalResultStr} TimeTaken: {GetString(timeTaken, unit)}");
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteException(taskName, ex);
        //        var timeTaken = DateTime.Now - startDate;
        //        WriteLogComplete($"Error. TimeTaken: {GetString(timeTaken, unit)}");
        //        return default;
        //    }
        //}
        //public Task<TResult> RunTaskAsync<TResult>(string taskName, Func<Task<TResult>> function, Func<TResult, string> getAdditionalResult, TimeUnit unit = DEFAULT_UNIT)
        //    => RunTaskAsync(taskName, function, getAdditionalResult, result => "Successful", unit);
        //public Task<TResult> RunTaskAsync<TResult>(string taskName, Func<Task<TResult>> function, TimeUnit unit = DEFAULT_UNIT)
        //    => RunTaskAsync(taskName, function, result => "", unit);
        #endregion

    }
}
