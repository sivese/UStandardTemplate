using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Std.Common.Pattern
{
    public class Pool<T> : IEnumerable where T : IResettable
    {
        public List<T> members = new();
        public HashSet<T> unavailable = new();
        
        private IFactory<T> factory;
    
        public Pool(IFactory<T> factory) : this(factory, 5) { }

        public Pool(IFactory<T> factory, int poolSize)
        {
            this.factory = factory;

            for(var i = 0; i < poolSize; i++) { Create(); }
        }

        public T Allocate()
        {
            foreach (var mem in members)
            {
                if (unavailable.Contains(mem)) continue;

                unavailable.Add(mem);

                return mem;
            }

            var newMember = Create();
            unavailable.Add(newMember);

            return newMember;
        }

        public void Release(T member)
        {
            member.Reset();
            unavailable.Remove(member);
        }

        private T Create()
        {
            members.Add(factory.Create());

            return members.Last();
        }

        public IEnumerator GetEnumerator() => members.GetEnumerator();
    }
}