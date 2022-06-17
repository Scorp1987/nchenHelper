using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace nchen.Tasks
{
    public interface ILoopTask
    {
        string Name { get; }
    }

    public static class ILoopTaskExtension
    {
        public static IEnumerable GetArray(this ILoopTask task, Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(task.Name)) throw new ArgumentException($"Empty {nameof(task.Name)} is not allowed for '{task.GetType()}'");

            object obj;
            try { obj = data.GetDeepPropertyValue(task.Name); }
            catch (Exception ex) { throw new ArgumentException($"'{task.Name}' is not a correct path", ex); }

            if (!(obj is IEnumerable arr)) throw new ArgumentException($"'{task.Name}' is '{obj.GetType()}' type and not an array");
            return arr;
        }
    }
}
