using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public static class PropertyChangingEventArgsExtension
    {
        public static bool GetCancel(this PropertyChangingEventArgs e)
        {
            if (e is PropertyChangingCancelEventArgs args) return args.Cancel;
            else return false;
        }
        public static void SetCancel(this PropertyChangingEventArgs e, bool cancel)
        {
            if (e is PropertyChangingCancelEventArgs args) args.Cancel = cancel;
            else throw new NotSupportedException(
                $"{nameof(args.Cancel)} is only avalibale for {nameof(PropertyChangingCancelEventArgs)}." +
                $"Current type is {e.GetType().Name}");
        }


        public static T GetOriginalValue<T>(this PropertyChangingEventArgs e)
        {
            if (e is PropertyChangingCancelEventArgs<T> args) return args.OriginalValue;
            else throw new NotSupportedException(
                $"{nameof(args.OriginalValue)} is only avaliable for {nameof(PropertyChangingCancelEventArgs<T>)}." +
                $"Current type is {e.GetType().Name}");
        }
        public static object GetOriginalValue(this PropertyChangingEventArgs e)
        {
            var type = e.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PropertyChangingCancelEventArgs<>))
                return type.GetProperty("OriginalValue").GetValue(e);
            else throw new NotSupportedException(
                $"OriginalValue is not avaliable for {type.Name}");
        }

        public static T GetNewValue<T>(this PropertyChangingEventArgs e)
        {
            if (e is PropertyChangingCancelEventArgs<T> args) return args.NewValue;
            else throw new NotSupportedException(
                $"{nameof(args.NewValue)} is only avaliable for {nameof(PropertyChangingCancelEventArgs<T>)}." +
                $"Current type is {e.GetType().Name}");
        }
        public static object GetNewValue(this PropertyChangingEventArgs e)
        {
            var type = e.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PropertyChangingCancelEventArgs<>))
                return type.GetProperty("NewValue").GetValue(e);
            else throw new NotSupportedException(
                $"NewValue is not avaliable for {type.Name}");
        }
    }
}
