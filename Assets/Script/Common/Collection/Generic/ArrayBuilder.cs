using System;
using System.Diagnostics;

namespace Std.Common.Collection
{
    internal class ArrayBuilder<T>
    {
        private const int DEFAULT_CAPACITY = 4;

        private T[]? array;
        private int count;

        public ArrayBuilder() { }

        public ArrayBuilder(int capacity) : this()
        {
            Debug.Assert(capacity >= 0);

            if (capacity > 0) array = new T[capacity];
        }

        public int Capacity => array?.Length ?? 0;

        public T[]? Buffer => array;

        public int Count => count;

        public T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < count);
                return array![index];
            }
        }

        public void Add(T item)
        {
            if (count == Capacity) EnsureCapacity(count + 1);

            UncheckedAdd(item);
        }

        public T First()
        {
            Debug.Assert(count > 0);
            return array![0];
        }

        public T Last()
        {
            Debug.Assert(count > 0);
            return array![count - 1];
        }

        public T[] ToArray()
        {
            if (count == 0) return Array.Empty<T>();

            Debug.Assert(array != null);

            var result = array;

            if(count < result.Length)
            {
                result = new T[count];
                Array.Copy(array, result, count);
            }

#if DEBUG
            count = -1;
            array = null;
#endif

            return result;
        }

        public void UncheckedAdd(T item)
        {
            Debug.Assert(count < Capacity);

            array![count] = item;
        }

        public void EnsureCapacity(int minimum)
        {
            Debug.Assert(minimum > Capacity);

            var capacity = Capacity;
            var nextCapacity = capacity == 0 ? DEFAULT_CAPACITY : 2 * capacity;

            if((uint) nextCapacity > int.MaxValue) // No guarantee that value is true max size.
            {
                nextCapacity = Math.Max(capacity + 1, int.MaxValue);
            }

            nextCapacity = Math.Max(nextCapacity, minimum);

            var next = new T[nextCapacity];

            if (count > 0) Array.Copy(array!, next, count);

            array = next;
        }
    }
}
