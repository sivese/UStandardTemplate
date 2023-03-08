
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using System.Collections;
using System.Collections.Generic;

using Std.Collection.Generic;

namespace Std.Collection.Tree
{
    [DebuggerDisplay("Count = {Count} ")]
    public class PriorityQueue<E, P> //element or value, priority
    {
        private (E Element, P TPriority)[] nodes;
        private readonly IComparer<P>? comparer;

        private UnorderedItemsCollection? unorderedItems;

        private int size;
        private int version;

        private const int Arity = 4;
        private const int Log2Arity = 2;

#if DEBUG
        static PriorityQueue()
        {
            Debug.Assert(Log2Arity > 0 && Math.Pow(2, Log2Arity) == Arity);
        }
#endif
        public PriorityQueue()
        {
            nodes = Array.Empty<(E, P)>();
            comparer = InitializeComparer(null);
        }

        public PriorityQueue(int initialCapacity) : this(initialCapacity, comparer : null)
        {

        }

        public PriorityQueue(IComparer<P>? comparer)
        {
            nodes = Array.Empty<(E, P)>();
            comparer = InitializeComparer(comparer);
        }

        public PriorityQueue(int initialCapacity, IComparer<P>? comparer)
        {
            if(initialCapacity < 0) throw new ArgumentOutOfRangeException();

            nodes = new (E, P)[initialCapacity];
            comparer = InitializeComparer(comparer);
        }

        public PriorityQueue(IEnumerable<(E Element, P Priority)> items) : this(items, comparer : null)
        {

        }

        public PriorityQueue(IEnumerable<(E Element, P Priority)> items, IComparer<P>? comparer)
        {
            if (items == null) throw new ArgumentNullException();

            nodes = EnumerableHelpers.ToArray(items, out size);
            comparer = InitializeComparer(comparer);

            if (size > 1) Heapify();
        }

        public int Count => size;

        public IComparer<P> Comparer => comparer ?? Comparer<P>.Default;

        public UnorderedItemsCollection UnorderedItems => unorderedItems ??= new UnorderedItemsCollection(this);

        public void Enqueue(E element, P priority)
        {
            var currentSize = size;
            version++;

            if (nodes.Length == currentSize) Grow(currentSize + 1);

            size = currentSize + 1;

            if (comparer == null) MoveUpDefaultComparer((element, priority), currentSize);
            else MoveUpCustomComparer((element, priority), currentSize);
        }

        public E Peek()
        {
            if (size == 0) throw new InvalidOperationException(); //plz insert error message here

            return nodes[0].Element;
        }

        public E Dequeue()
        {
            if (size == 0) throw new InvalidOperationException();

            var element = nodes[0].Element;
            RemoveRootNode();

            return element;
        }

        public E DequeueEnqueue(E element, P priority)
        {
            if (size == 0) throw new InvalidOperationException();

            (E Element, P Priority) root = nodes[0];

            if(comparer == null)
            {
                if (Comparer<P>.Default.Compare(priority, root.Priority) > 0) MoveDownDefaultComparer((element, priority), 0);
                else nodes[0] = (element, priority);
            }
            else
            {
                if (comparer.Compare(priority, root.Priority) > 0) MoveDownCustomComparer((element, priority), 0);
                else nodes[0] = (element, priority);
            }

            version++;
            return root.Element;
        }

        public bool TryDequeue([MaybeNullWhen(false)] out E element, [MaybeNullWhen(false)] out P priority)
        {
            if(size != 0)
            {
                (element, priority) = nodes[0];
                RemoveRootNode();
                return true;
            }

            element = default;
            priority = default;

            return false;
        }

        public bool TryPeek([MaybeNullWhen(false)] out E element, [MaybeNullWhen(false)] out P priority)
        {
            if(size != 0)
            {
                (element, priority) = nodes[0];
                return true;
            }

            element = default;
            priority = default;

            return false;
        }

        public E EnqueueDequeue(E element, P priority)
        {
            if(size != 0)
            {
                (E Element, P Priority) root = nodes[0];

                if(comparer == null)
                {
                    if(Comparer<P>.Default.Compare(priority, root.Priority) > 0)
                    {
                        MoveDownDefaultComparer((element, priority), 0);
                        version++;

                        return root.Element;
                    }
                }
                else
                {
                    if(comparer.Compare(priority, root.Priority) > 0)
                    {
                        MoveDownCustomComparer((element, priority), 0);
                        version++;

                        return root.Element;
                    }
                }
            }

            return element;
        }

        public void EnqueueRange(IEnumerable<(E Element, P Priority)> items)
        {
            if(items == null) throw new ArgumentException();

            var count = 0;
            var collection = items as ICollection<(E Element, P Priority)>;

            if (collection is not null && (count = collection.Count) > nodes.Length - size) Grow(checked(size + count));

            if(size == 0)
            {
                if (collection is not null)
                {
                    collection.CopyTo(nodes, 0);
                    size = count;
                }
                else
                {
                    var i = 0;
                    var nodes = this.nodes;

                    foreach(var (element, priority) in items)
                    {
                        if(nodes.Length == i)
                        {
                            Grow(i + 1);
                            nodes = this.nodes;
                        }

                        nodes[i++] = (element, priority);
                    }

                    size = i;
                }

                version++;

                if(size > 1) Heapify();
            }
            else
            {
                foreach (var (element, priority) in items) Enqueue(element, priority);
            }
        }

        public void EnqueueRange(IEnumerable<E> elements, P priority)
        {
            if(elements == null) throw new ArgumentNullException();

            var count = 0;

            if (elements is ICollection<E> collection && (count = collection.Count) > nodes.Length - size) Grow(checked(size + count));

            if(size == 0)
            {
                var i = 0;

                var nodes = this.nodes;

                foreach(var element in elements)
                {
                    if(nodes.Length == i)
                    {
                        Grow(i + 1);
                        nodes = this.nodes;
                    }

                    nodes[i++] = (element, priority);
                }

                size = i;
                version++;

                if (i > 1) Heapify();
            }
            else
            {
                foreach (var element in elements) Enqueue(element, priority);
            }
        }

        public void Clear()
        {
            if(RuntimeHelpers.IsReferenceOrContainsReferences<(E, P)>())
            {
                Array.Clear(nodes, 0, size);
            }

            size = 0;
            version++;
        }

        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException();

            if(nodes.Length < capacity)
            {
                Grow(capacity);
                version++;
            }

            return nodes.Length;
        }

        public void TrimExcess()
        {
            var threshold = (int) nodes.Length * 0.9;

            if(size < threshold)
            {
                Array.Resize(ref nodes, size);
                version++;
            }
        }

        private void Grow(int minCapacity)
        {
            Debug.Assert(nodes.Length < minCapacity);

            const int GROW_FACTOR = 2;
            const int MINIMUM_GROW = 4;

            var newCapacity = GROW_FACTOR * nodes.Length;

            if((uint) newCapacity > int.MaxValue) newCapacity = int.MaxValue;

            newCapacity = Math.Max(newCapacity, nodes.Length + MINIMUM_GROW);

            if (newCapacity < minCapacity) newCapacity = minCapacity;

            Array.Resize(ref nodes, newCapacity);
        }

        private void RemoveRootNode()
        {
            var lastNodeIndex = --size;
            version++;

            if(lastNodeIndex > 0)
            {
                var lastNode = nodes[lastNodeIndex];

                if (comparer == null) MoveDownDefaultComparer(lastNode, 0);
                else MoveDownCustomComparer(lastNode, 0);
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<(E, P)>()) nodes[lastNodeIndex] = default;
        }

        private static int GetParentIndex(int index) => (index - 1) >> Log2Arity;

        private static int GetFirstChildIndex(int index) => (index << Log2Arity) + 1;

        private void Heapify()
        {
            var nodes = this.nodes;

            int lastParentWithChildren = GetParentIndex(size - 1);

            if(comparer == null)
            {
                for (var index = lastParentWithChildren; index >= 0; --index) MoveDownDefaultComparer(nodes[index], index);
            }
            else
            {
                for (var index = lastParentWithChildren; index >= 0; --index) MoveDownCustomComparer(nodes[index], index);
            }
        }

        private void MoveUpDefaultComparer((E Element, P Priority) node, int nodeIndex)
        {
            Debug.Assert(comparer is null);
            Debug.Assert(0 <= nodeIndex && nodeIndex < size);

            var nodes = this.nodes;

            while(nodeIndex > 0)
            {
                var parentIndex = GetParentIndex(nodeIndex);
                (E Element, P Priority) parent = nodes[parentIndex];

                if(Comparer<P>.Default.Compare(node.Priority, parent.Priority) < 0)
                {
                    nodes[nodeIndex] = parent;
                    nodeIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }

            nodes[nodeIndex] = node;
        }

        private void MoveUpCustomComparer((E Element, P Priority) node, int nodeIndex)
        {
            Debug.Assert(this.comparer is not null);
            Debug.Assert(0 <= nodeIndex && nodeIndex < size);

            var comparer = this.comparer;
            var nodes = this.nodes;

            while(nodeIndex > 0)
            {
                var parentIndex = GetParentIndex(nodeIndex);
                (E Element, P Priority) parent = nodes[parentIndex];

                if(comparer.Compare(node.Priority, parent.Priority) < 0)
                {
                    nodes[nodeIndex] = parent;
                    nodeIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }

            nodes[nodeIndex] = node;
        }

        private void MoveDownDefaultComparer((E Element, P Priority) node, int nodeIndex)
        {
            Debug.Assert(comparer is null);
            Debug.Assert(0 <= nodeIndex && nodeIndex < this.size);

            var nodes = this.nodes;
            var size = this.size;

            var i = 0;

            while((i = GetFirstChildIndex(nodeIndex)) < size)
            {
                (E Element, P Priority) minChild = nodes[i];
                var minChildIndex = i;

                var childIndexUpperBound = Math.Min(i + Arity, size);

                while(++i < childIndexUpperBound)
                {
                    (E Element, P Priority) nextChild = nodes[i];

                    if(Comparer<P>.Default.Compare(nextChild.Priority, minChild.Priority) < 0)
                    {
                        minChild = nextChild;
                        minChildIndex = i;
                    }
                }

                if (Comparer<P>.Default.Compare(node.Priority, minChild.Priority) <= 0) break;

                nodes[nodeIndex] = minChild;
                nodeIndex = minChildIndex;
            }

            nodes[nodeIndex] = node;
        }

        private void MoveDownCustomComparer((E Element, P Priority) node, int nodeIndex)
        {
            Debug.Assert(this.comparer is null);
            Debug.Assert(0 <= nodeIndex && nodeIndex < this.size);

            var comparer = this.comparer;
            var nodes = this.nodes;

            var size = this.size;
            var i = 0;

            while((i = GetFirstChildIndex(nodeIndex)) < size)
            {
                (E Element, P Priority) minChild = nodes[i];
                var minChildIndex = i;

                var childIndexUpperBound = Math.Min(i + Arity, size);

                while(++i < childIndexUpperBound)
                {
                    (E Element, P Priority) nextChild = nodes[i];

                    if(comparer.Compare(nextChild.Priority, minChild.Priority) < 0)
                    {
                        minChild = nextChild;
                        minChildIndex = i;
                    }
                }

                if (comparer.Compare(node.Priority, minChild.Priority) <= 0) break;

                nodes[nodeIndex] = minChild;
                nodeIndex = minChildIndex;
            }

            nodes[nodeIndex] = node;
        }

        private static IComparer<P>? InitializeComparer(IComparer<P>? comparer)
        {
            if(typeof(P).IsValueType)
            {
                if (comparer == Comparer<P>.Default) return null;

                return comparer;
            }

            return comparer ?? Comparer<P>.Default;
            // null check operator
            // if target is null, then return right value
        }

        [DebuggerDisplay("Count = {Count}")]
        public sealed class UnorderedItemsCollection : IReadOnlyCollection<(E Element, P Priority)>, ICollection
        {
            internal readonly PriorityQueue<E, P> queue;

            internal UnorderedItemsCollection(PriorityQueue<E, P> queue) => this.queue = queue;

            public int Count => queue.size;

            public bool IsSynchronized => throw new NotImplementedException();

            public object SyncRoot => throw new NotImplementedException();

            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<(E Element, P Priority)> GetEnumerator()
            {
                throw new System.NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new System.NotImplementedException();
            }
        }
    }

}
