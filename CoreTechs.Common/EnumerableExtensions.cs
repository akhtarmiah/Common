﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreTechs.Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SkipWhileNot<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.SkipWhile(x => !predicate(x));
        }

        public static IEnumerable<T> TakeWhileNot<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.TakeWhile(x => !predicate(x));
        }

        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Where(x => !predicate(x));
        }

        public static IEnumerable<IEnumerable<T>> Buffer<T>(this IEnumerable<T> source, int bufferSize)
        {
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                    yield return YieldBufferElements(enumerator, bufferSize - 1);
        }

        private static IEnumerable<T> YieldBufferElements<T>(IEnumerator<T> source, int bufferSize)
        {
            yield return source.Current;
            for (var i = 0; i < bufferSize && source.MoveNext(); i++)
                yield return source.Current;
        }

        public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        /// <summary>
        /// Shuffles an enumeration of items.
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng = null)
        {
            return source.ToArray().Shuffle(rng);
        }

        /// <summary>
        /// Shuffles a collection of items.
        /// </summary>
        /// <thanks to="Jon Skeet">http://stackoverflow.com/a/1287572/64334</thanks>
        public static IEnumerable<T> Shuffle<T>(this IList<T> list, Random rng = null)
        {
            rng = rng ?? RNG.Instance;
            var elements = new T[list.Count];
            list.CopyTo(elements, 0);
            for (var i = elements.Length - 1; i >= 0; i--)
            {
                // Swap element "i" with a random earlier element it (or itself)
                // ... except we don't really need to swap it fully, as we can
                // return it immediately, and afterwards it's irrelevant.
                var swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        /// <summary>
        /// Yields an object indefinetely.
        /// </summary>
        public static IEnumerable<T> Repeat<T>(this T source)
        {
            while (true) yield return source;
        }

        /// <summary>
        /// Repeatedly invokes the delegate and yields it's result.
        /// </summary>
        public static IEnumerable<T> InvokeRepeatedly<T>(this Func<T> func)
        {
            while (true) yield return func();
        }

        /// <thanks to="Jon Skeet">http://stackoverflow.com/a/648240/64334</thanks>
        public static T RandomElement<T>(this IEnumerable<T> source, Random rng = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            rng = rng ?? RNG.Instance;

            var list = source as IList<T>;
            if (list != null) return list.RandomElement(rng);

            var current = default(T);
            var count = 0;
            foreach (var element in source)
            {
                count++;
                if (rng.Next(count) == 0)
                    current = element;
            }

            if (count == 0)
                throw new InvalidOperationException("Sequence was empty");

            return current;
        }

        // more efficient for collections with an indexer
        public static T RandomElement<T>(this IList<T> list, Random rng = null)
        {
            if (list == null) throw new ArgumentNullException("list");

            if (list.Count == 0)
                throw new InvalidOperationException("List was empty");

            rng = rng ?? RNG.Instance;
            var i = rng.Next(list.Count);
            var element = list[i];
            return element;
        }
    }
}
