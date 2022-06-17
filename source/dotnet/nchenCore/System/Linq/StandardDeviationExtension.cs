using System;
using System.Collections.Generic;

namespace System.Linq
{
    public static class StandardDeviationExtension
    {
        private static double StandardDeviation<TSource>(this IEnumerable<TSource> source, VarianceType type, Func<TSource, double> selector) => Math.Sqrt(source.Variance(type, selector));
        private static double? StandardDeviation<TSource>(this IEnumerable<TSource?> source, VarianceType type, Func<IEnumerable<TSource?>, VarianceType, double?> standardDeviationFunc)
            where TSource : struct
        {
            var query = source.Where(v => v.HasValue).Select(v => v.Value);
            if (query.Count() < 1) return null;
            return standardDeviationFunc(source, type);
        }


        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="decimal"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="decimal"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation(this IEnumerable<decimal> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, v => (double)v);
        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="double"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="double"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation(this IEnumerable<double> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, v => v);
        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="float"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="float"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation(this IEnumerable<float> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, v => v);
        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="int"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="int"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation(this IEnumerable<int> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, v => v);
        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="long"/> values
        /// </summary>
        /// <param name="source">A sequence of <see cref="long"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation(this IEnumerable<long> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, v => v);


        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="decimal"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="decimal"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation(this IEnumerable<decimal?> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, StandardDeviation);
        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="double"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="double"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation(this IEnumerable<double?> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, StandardDeviation);
        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="float"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="float"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation(this IEnumerable<float?> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, StandardDeviation);
        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="int"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="int"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation(this IEnumerable<int?> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, StandardDeviation);
        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="long"/> values
        /// </summary>
        /// <param name="source">A sequence of nullable <see cref="long"/> values to calculate the standard deviation of.</param>
        /// <param name="type">type of variance</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation(this IEnumerable<long?> source, VarianceType type = VarianceType.Population) => StandardDeviation(source, type, StandardDeviation);


        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="decimal"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector) => source.Select(selector).StandardDeviation();
        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="double"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector) => source.Select(selector).StandardDeviation();
        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="float"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector) => source.Select(selector).StandardDeviation();
        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="int"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector) => source.Select(selector).StandardDeviation();
        /// <summary>
        /// Computes the standard deviation of a sequence of <see cref="long"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values.</returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">source contains no elements.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector) => source.Select(selector).StandardDeviation();


        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="decimal"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector) => source.Select(selector).StandardDeviation();
        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="double"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector) => source.Select(selector).StandardDeviation();
        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="float"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector) => source.Select(selector).StandardDeviation();
        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="int"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector) => source.Select(selector).StandardDeviation();
        /// <summary>
        /// Computes the standard deviation of a sequence of nullable <see cref="long"/> values that are obtained by
        /// invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to calculate the standard deviation of.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation of the sequence of values, or <see langword="null"/> if 
        /// the source sequence is empty or contains only values that are <see langword="null"/></returns>
        /// <exception cref="ArgumentNullException">source is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">source contains more than <see cref="int.MaxValue"/> elements.</exception>
        /// <exception cref="NotImplementedException">type is not a defined SequenceType.</exception>
        public static double? StandardDeviation<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector) => source.Select(selector).StandardDeviation();
    }
}
