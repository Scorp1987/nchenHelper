namespace System
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// Convert the value of current <see cref="DateTime"/> object
        /// to round down to Second and ignore the milliseconds.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>
        /// <see cref="DateTime"/> object that is rounded down to Second
        /// and ignore the milliseconds.
        /// </returns>
        public static DateTime ToRoundDownToSecond(this DateTime dt)
            => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);

        /// <summary>
        /// Convert the value of current <see cref="DateTime"/> object
        /// to round down to Millisecond and ignore the nanoseconds.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>
        /// <see cref="DateTime"/> object that is rounded down to Millisecond
        /// and ignore the nanoseconds.
        /// </returns>
        public static DateTime ToRoundDownToMilliSecond(this DateTime dt)
            => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

        /// <summary>
        /// Convert the value of current <see cref="DateTime"/> object to
        /// string value to use in sqlStatement
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>string value to use in sqlStatement</returns>
        public static string ToSqlValueString(this DateTime dt) => $"'{dt.ToRoundDownToMilliSecond():yyyy-MM-dd HH:mm:ss.fff}'";
    }
}
