using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static class MedianExtension
    {
        private static TReturn Median<Tsource, TReturn>(this IEnumerable<Tsource> source, Func<Tsource,TReturn> convertFunc, Func<Tsource, Tsource, TReturn>averageFunction)
        {
            var values = source.OrderBy(s => s).ToArray();
            var count = values.Length;
            if (count == 0) throw new InvalidOperationException($"Can't find median for empty array");
            if (count.IsOdd())
                return convertFunc(values[count / 2]);
            else
            {
                var value1 = values[(count / 2) - 1];
                var value2 = values[(count / 2)];
                return averageFunction(value1, value2);
            }
        }
        private static TReturn? Median<TSource, TReturn>(this IEnumerable<TSource?> source, Func<IEnumerable<TSource>, TReturn> medianFunc)
            where TSource : struct
            where TReturn : struct
        {
            var query = source.Where(v => v.HasValue).Select(v => v.Value);
            if (query.Count() < 1) return null;
            return medianFunc(query);
        }


        /// <summary>
        /// Computes the median of a sequence of <see cref="decimal"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="decimal"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static decimal Median(this IEnumerable<decimal> source) => Median(source, v => v, (v1, v2) => (v1 + v2) / 2);
        /// <summary>
        /// Computes the median of a sequence of <see cref="double"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="double"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double Median(this IEnumerable<double> source) => Median(source, v => v, (v1, v2) => (v1 + v2) / 2);
        /// <summary>
        /// Computes the median of a sequence of <see cref="float"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="float"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static float Median(this IEnumerable<float> source) => Median(source, v => v, (v1, v2) => (v1 + v2) / 2);
        /// <summary>
        /// Computes the median of a sequence of <see cref="int"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="int"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double Median(this IEnumerable<int> source) => Median(source, v => v, (v1, v2) => 0.5d * (v1 + v2));
        /// <summary>
        /// Computes the median of a sequence of <see cref="long"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="long"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double Median(this IEnumerable<long> source) => Median(source, v => v, (v1, v2) => 0.5d * (v1 + v2));


        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="decimal"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="decimal"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/></exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static decimal? Median(this IEnumerable<decimal?> source) => Median(source, Median);
        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="double"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="double"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/></exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double? Median(this IEnumerable<double?> source) => Median(source, Median);
        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="float"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="float"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/></exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static float? Median(this IEnumerable<float?> source) => Median(source, Median);
        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="int"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="int"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/></exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double? Median(this IEnumerable<int?> source) => Median(source, Median);
        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="long"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="long"/> values to calculate the median of.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/></exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double? Median(this IEnumerable<long?> source) => Median(source, Median);


        /// <summary>
        /// Computes the median of a sequence of <see cref="decimal"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static decimal Median<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector) => source.Select(selector).Median();
        /// <summary>
        /// Computes the median of a sequence of <see cref="double"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double Median<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector) => source.Select(selector).Median();
        /// <summary>
        /// Computes the median of a sequence of <see cref="float"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static float Median<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) => source.Select(selector).Median();
        /// <summary>
        /// Computes the median of a sequence of <see cref="int"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double Median<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) => source.Select(selector).Median();
        /// <summary>
        /// Computes the median of a sequence of <see cref="long"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double Median<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector) => source.Select(selector).Median();


        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="decimal"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if the source sequence is empty
        /// or contains only values that are <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static decimal? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector) => source.Select(selector).Median();
        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="double"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if the source sequence is empty
        /// or contains only values that are <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector) => source.Select(selector).Median();
        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="float"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if the source sequence is empty
        /// or contains only values that are <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static float? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector) => source.Select(selector).Median();
        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="int"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if the source sequence is empty
        /// or contains only values that are <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector) => source.Select(selector).Median();
        /// <summary>
        /// Computes the median of a sequence of nullable <see cref="long"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the median of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The median of the sequence of values, or <see langword="null"/> if the source sequence is empty
        /// or contains only values that are <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException">source is null.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        public static double? Median<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector) => source.Select(selector).Median();
    }
}
