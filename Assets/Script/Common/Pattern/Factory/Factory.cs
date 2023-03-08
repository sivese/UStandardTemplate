using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Std.Common.Pattern
{
    public class Factory<T> : IFactory<T> where T : new()
    {
        public T Create() => new T();
    }
}
