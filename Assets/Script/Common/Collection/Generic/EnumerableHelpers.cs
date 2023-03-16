using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Std.Common.Collection;

namespace Std.Collection.Generic
{
    internal static partial class EnumerableHelpers
    {
        internal static IEnumerator<T> GetEmptyEnumerator<T>() => ((IEnumerable<T>)Array.Empty<T>()).GetEnumerator();

        internal static void Copy<T>(IEnumerable<T> source, T[] array, int arrayIndex, int count)
        {
            Debug.Assert(source != null);
            Debug.Assert(arrayIndex >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(array.Length - arrayIndex >= count);

            if (source is ICollection<T> collection)
            {
                Debug.Assert(collection.Count == count);
                collection.CopyTo(array, arrayIndex);
                return;
            }

            IterativeCopy(source, array, arrayIndex, count);
        }

        internal static void IterativeCopy<T>(IEnumerable<T> source, T[] array, int arrayIndex, int count)
        {
            //check argument condition
            Debug.Assert(source != null && !(source is ICollection<T>));
            Debug.Assert(arrayIndex >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(array.Length - arrayIndex >= count);

            var endIndex = arrayIndex + count;

            foreach (T item in source) array[arrayIndex++] = item;

            Debug.Assert(arrayIndex == endIndex);
        }

        internal static T[] ToArray<T>(IEnumerable<T> source)
        {
            Debug.Assert(source != null);

            if(source is ICollection<T> collection)
            {
                int count = collection.Count;

                if(count == 0) return Array.Empty<T>();

                var result = new T[count];
                collection.CopyTo(result, arrayIndex: 0);
                
                return result;
            }

            var builder = new LargeArrayBuilder<T>();
            builder.AddRange(source);

            return source.ToArray();
        }

        internal static T[] ToArray<T>(IEnumerable<T> source, out int length)
        {
            if(source is ICollection<T> ic)
            {
                var count = ic.Count;

                if(count != 0)
                {
                    var arr = new T[count];

                    ic.CopyTo(arr, 0);
                    length = count;

                    return arr;
                }
            }
            else
            {
                using(var en = source.GetEnumerator())
                {
                    if(en.MoveNext())
                    {
                        const int DEFAULT_CAPACITY = 4;

                        var arr = new T[DEFAULT_CAPACITY];
                        arr[0] = en.Current;

                        var count = 1;

                        while(en.MoveNext())
                        {
                            if (count == arr.Length)
                            {
                                var newLength = count << 1;

                                if((uint) newLength > int.MaxValue)
                                {
                                    newLength = int.MaxValue <= count ? count + 1 : int.MaxValue;
                                }

                                Array.Resize(ref arr, newLength);
                            }

                            arr[count++] = en.Current;
                        }

                        length = count;

                        return arr;
                    }
                }
            }

            length = 0;

            return Array.Empty<T>();
        }
    }
}
