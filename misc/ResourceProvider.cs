using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace NipaGameKit
{
    [Serializable]
    public class ResourceProvider<T> where T : class
    {
        [Serializable]
        public class Resource<T>
        {
            public string[] ids;
            public T res;
        }

        [SerializeField] private List<Resource<T>> resources;

        public T GetResource(string id)
        {
            var resource = resources.Find(r => r.ids.Any(v => v == id));
            if (resource == null)
            {
                Debug.LogError($"Resource not found: {id}");
                return null;
            }

            return resource.res;
        }
    }
}