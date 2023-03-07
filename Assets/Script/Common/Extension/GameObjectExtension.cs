using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Std.Common.Extension
{
    public static class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component;

            gameObject.TryGetComponent(out component);

            if (component == null) component = gameObject.AddComponent<T>();

            return component;
        }
    }
}
