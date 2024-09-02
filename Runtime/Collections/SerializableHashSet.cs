#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Collections
{
    [Serializable]
    public sealed class SerializableHashSet<T> : ISet<T>, ISerializationCallbackReceiver
    {
        private HashSet<T> set = new HashSet<T>();

        [SerializeField]
        private List<T> data = new List<T>();

        public SerializableHashSet() { }

        public SerializableHashSet(IEnumerable<T> collection) => set = new HashSet<T>(collection);

        public void OnBeforeSerialize()
        {
            data.Clear();

            foreach (var tData in set)
            {
                data.Add(tData);
            }
        }

        public void OnAfterDeserialize()
        {
            set = new HashSet<T>();

            foreach (var tData in data)
            {
                set.Add(tData);
            }
        }

        public int Count => set.Count;

        public bool IsReadOnly => ((ISet<T>)set).IsReadOnly;

        public bool Add(T item) => set.Add(item);

        public void Clear() => set.Clear();

        public bool Contains(T item) => set.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => set.CopyTo(array, arrayIndex);

        public void ExceptWith(IEnumerable<T> other) => set.ExceptWith(other);

        public void IntersectWith(IEnumerable<T> other) => set.IntersectWith(other);

        public bool IsProperSubsetOf(IEnumerable<T> other) => set.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => set.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => set.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => set.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => set.Overlaps(other);

        public bool Remove(T item) => set.Remove(item);

        public bool SetEquals(IEnumerable<T> other) => set.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<T> other) => set.SymmetricExceptWith(other);

        public void UnionWith(IEnumerable<T> other) => set.UnionWith(other);

        public IEnumerator<T> GetEnumerator() => set.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((ISet<T>)set).GetEnumerator();

        void ICollection<T>.Add(T item) => ((ICollection<T>)set).Add(item);

    }
}
#endif
