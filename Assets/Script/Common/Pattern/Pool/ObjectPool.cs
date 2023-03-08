using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Std.Common.Pattern
{
    public class ObjectPool
    { 
        public const int CREATE_COUNT = 8;

        protected GameObject parent;
        protected GameObject prefab;

        protected List<GameObject> objects;

        public ObjectPool(GameObject prefab, GameObject parent = null, int count = CREATE_COUNT)
        {
            this.parent = parent;
            this.prefab = prefab;

            objects = new();

            foreach(var _ in Enumerable.Range(0, count))
            {
                var tmp = CreateObject();
                tmp.SetActive(false);

                objects.Add(tmp);
            }
        }

        public GameObject GetObject()
        {
            foreach(var obj in objects)
            {
                if (obj.activeSelf) continue;

                obj.SetActive(true);
                return obj;
            }

            return CreateObject();
        }

        public GameObject CreateObject()
        {
            var tmp = GameObject.Instantiate(prefab);

            if (parent != null)
            {
                tmp.transform.parent = parent.transform;
            }

            objects.Add(tmp);

            return tmp;
        }
    }
}
