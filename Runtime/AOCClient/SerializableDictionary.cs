using System;
using System.Collections.Generic;
using UnityEngine;

namespace JTuresson.AdventOfCode.AOCClient
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>,
        ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new List<TKey>();

        [SerializeField] private List<TValue> values = new List<TValue>();

        // save the dictionary to lists
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (var (key, value) in this)
            {
                keys.Add(key);
                values.Add(value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Count != values.Count)
                throw new System.Exception(string.Format(
                    "there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

            for (var i = 0; i < keys.Count; i++)
                Add(keys[i], values[i]);
        }
    }
}