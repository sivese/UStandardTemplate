using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Std.Common.Collection
{
    // Helper type for building dynamically-sized arrays while minimizing allocations and copying.
    internal struct LargeArrayBuilder<T>
    {
        private const int STARTING_CAPACITY = 4;
        private const int RESIZE_LIMIT = 8;

        private readonly int MaxCapacity;
        private T[] first;

        private ArrayBuilder<T[]> buffers;
        private T[] current;
        private int index;
        private int count;

        public LargeArrayBuilder(object unused = null) : this(maxCapacity : int.MaxValue) { }

        public LargeArrayBuilder(int maxCapacity)
        {
            Debug.Assert(maxCapacity >= 0);

            this = default;
            first = current = Array.Empty<T>();
            MaxCapacity = maxCapacity;
        }

        public int Count => count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            Debug.Assert(MaxCapacity > count);

            var idx = index;
            var cur = current;

            if((uint) idx >= (uint) cur.Length)
            {
                AddWithBufferAllocation(item);
            }
            else
            {
                cur[idx] = item;
                index = idx + 1;
            }

            count++;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithBufferAllocation(T item)
        {
            AllocateBuffer();
            current[index++] = item;
        }

        public void AddRange(IEnumerable<T> items)
        {
            Debug.Assert(items != null);

            using(var enumerator = items.GetEnumerator())
            {
                var destination = current;
                var idx = index;

                while(enumerator.MoveNext())
                {
                    var item = enumerator.Current;

                    if ((uint)index >= (uint)destination.Length) AddWithBufferAllocation(item, ref destination, ref idx);
                    else destination[idx] = item;
                
                    idx++;
                }

                count += idx - index;
                index = idx;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithBufferAllocation(T item, ref T[] destination, ref int index)
        {
            count += index - this.index;
            this.index = index;

            AllocateBuffer();

            destination = current;
            index = this.index;

            current[index] = item;
        }

        private void AllocateBuffer()
        {
            Debug.Assert((uint) MaxCapacity > (uint) count);
            Debug.Assert(index == current.Length, $"{nameof(AllocateBuffer)} was called, but there's more space.");

            if ((uint) count < (uint) RESIZE_LIMIT)
            {
                Debug.Assert(current == first && count == first.Length);

                int nextCapacity = Math.Min(count == 0 ? STARTING_CAPACITY : count * 2, MaxCapacity);

                current = new T[nextCapacity];
                
                Array.Copy(first, current, count);
                first = current;
            }
            else
            {
                Debug.Assert(MaxCapacity > RESIZE_LIMIT);
                Debug.Assert(count == RESIZE_LIMIT ^ current != first);

                int nextCapacity;

                if (count == RESIZE_LIMIT)
                {
                    nextCapacity = RESIZE_LIMIT;
                }
                else
                {
                    Debug.Assert(count >= RESIZE_LIMIT * 2);
                    Debug.Assert(count == current.Length * 2);

                    buffers.Add(current);
                    
                    nextCapacity = Math.Min(count, MaxCapacity - count);
                }

                current = new T[nextCapacity];
                index = 0;
            }
        }
    }
}
