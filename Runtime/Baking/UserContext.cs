#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter
{
    public struct UserContext
    {
        private Dictionary<Type, ScriptableObject> userData;

        internal static UserContext Create(List<ScriptableObject> userContext)
        {
            var userData = new Dictionary<Type, ScriptableObject>();

            foreach (var data in userContext)
            {
                userData.Add(data.GetType(), data);
            }

            return new UserContext() { userData = userData };
        }

        public bool TryGet<T>(out T value) where T : ScriptableObject
        {
            if (userData.TryGetValue(typeof(T), out var data))
            {
                value = data as T;
                return true;
            }

            value = default;
            return false;
        }
    }
}
#endif
