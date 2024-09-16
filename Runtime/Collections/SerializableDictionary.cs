#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scellecs.Morpeh.EntityConverter.Collections
{
    [Serializable]
    public sealed class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        private Dictionary<TKey, TValue> dictionary;

        [SerializeField]
        private List<TKey> keys;

        [SerializeField]
        private List<TValue> values;

        public SerializableDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary = new Dictionary<TKey, TValue>();

            for (int i = 0; i != Math.Min(keys.Count, values.Count); i++)
            {
                dictionary.Add(keys[i], values[i]);
            }
        }

        public ICollection<TKey> Keys => dictionary.Keys;

        public ICollection<TValue> Values => dictionary.Values;

        public int Count => dictionary.Count;

        public bool IsReadOnly => ((IDictionary<TKey, TValue>)dictionary).IsReadOnly;

        public TValue this[TKey key] { get => dictionary[key]; set => dictionary[key] = value; }

        public void Add(TKey key, TValue value) => dictionary.Add(key, value);

        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        public bool Remove(TKey key) => dictionary.Remove(key);

        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item) => dictionary.Add(item.Key, item.Value);

        public void Clear() => dictionary.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>) dictionary).Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)dictionary).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)dictionary).Remove(item);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IDictionary<TKey, TValue>)dictionary).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<TKey, TValue>)dictionary).GetEnumerator();
    }
}
#endif
