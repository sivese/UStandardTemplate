using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Std.Common.Pattern
{
    public interface IFactory<T> { T Create(); }
}