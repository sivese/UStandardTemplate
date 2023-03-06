using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Std.Common.Pattern
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance = null;

        public static T GetInstance()
        {
            if (instance != null) return instance;

            var typeName = typeof(T).Name;
            var go = new GameObject(typeName);

            return go.AddComponent<T>();
        }

        public static bool IsNull() => instance == null;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);

            Init();
        }

        protected abstract void Init();

        private void OnDestroy()
        {
            instance = null;
        }
    }
}