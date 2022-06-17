using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static class VarianceExtension
    {
        internal static double Variance<TSource>(this IEnumerable<TSource> source, VarianceType type, Func<TSource, double> selector)
        {
            var average = source.Average(selector);
            var sum = source.Sum(s => Math.Pow(selector(s) - average, 2));
            var count = source.Count();
            return type switch
            {
                VarianceType.Population => sum / count,
                VarianceType.Sample => sum / (count - 1),
                _ => throw new NotImplementedException($"'{type}' is not Implemented")
            };
        }
        private static double? Variance<TSource>(this IEnumerable<TSource?> source, VarianceType type, Func<IEnumerable<TSource>, VarianceType, double> varianceFunc)
            where TSource : struct
        {
            var query = source.Where(v => v.HasValue).Select(v => v.Value);
            if (query.Count() < 1) return null;
            return varianceFunc(query, type);
        }


        /// <summary>
        /// Computes the variance of a sequence of <see cref="decimal"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="decimal"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance(this IEnumerable<decimal> source, VarianceType type = VarianceType.Population) => source.Variance(type, v => (double)v);
        /// <summary>
        /// Computes the variance of a sequence of <see cref="double"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="double"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance(this IEnumerable<double> source, VarianceType type = VarianceType.Population) => source.Variance(type, v => v);
        /// <summary>
        /// Computes the variance of a sequence of <see cref="float"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="float"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance(this IEnumerable<float> source, VarianceType type = VarianceType.Population) => source.Variance(type, v => v);
        /// <summary>
        /// Computes the variance of a sequence of <see cref="int"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="int"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance(this IEnumerable<int> source, VarianceType type = VarianceType.Population) => source.Variance(type, v => v);
        /// <summary>
        /// Computes the variance of a sequence of <see cref="long"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="long"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance(this IEnumerable<long> source, VarianceType type = VarianceType.Population) => source.Variance(type, v => v);


        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="decimal"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="decimal"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance(this IEnumerable<decimal?> source, VarianceType type = VarianceType.Population) => source.Variance(type, Variance);
        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="double"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="double"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance(this IEnumerable<double?> source, VarianceType type = VarianceType.Population) => source.Variance(type, Variance);
        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="float"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="float"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance(this IEnumerable<float?> source, VarianceType type = VarianceType.Population) => source.Variance(type, Variance);
        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="int"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="int"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance(this IEnumerable<int?> source, VarianceType type = VarianceType.Population) => source.Variance(type, Variance);
        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="long"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="long"/> values to calculate the variance of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance(this IEnumerable<long?> source, VarianceType type = VarianceType.Population) => source.Variance(type, Variance);


        /// <summary>
        /// Computes the variance of a sequence of <see cref="decimal"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector) => source.Select(selector).Variance();
        /// <summary>
        /// Computes the variance of a sequence of <see cref="double"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector) => source.Select(selector).Variance();
        /// <summary>
        /// Computes the variance of a sequence of <see cref="float"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) => source.Select(selector).Variance();
        /// <summary>
        /// Computes the variance of a sequence of <see cref="int"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) => source.Select(selector).Variance();
        /// <summary>
        /// Computes the variance of a sequence of <see cref="long"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector) => source.Select(selector).Variance();


        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="decimal"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector) => source.Select(selector).Variance();
        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="double"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector) => source.Select(selector).Variance();
        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="float"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector) => source.Select(selector).Variance();
        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="int"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector) => source.Select(selector).Variance();
        /// <summary>
        /// Computes the variance of a sequence of nullable <see cref="long"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the variance of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The variance of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? Variance<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector) => source.Select(selector).Variance();
    }
}
