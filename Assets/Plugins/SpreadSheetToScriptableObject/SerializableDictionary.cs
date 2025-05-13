using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;

namespace SpreadSheetToScriptableObject
{
    [Serializable]
    public class CustomKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public CustomKeyValuePair(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }
        public CustomKeyValuePair(KeyValuePair<TKey, TValue> pair)
        {
            this.Key = pair.Key;
            this.Value = pair.Value;
        }
    }

    [Serializable]
    public class SerializableDictionary<K, V>
        : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        public SerializableDictionary()
        {

        }
#if UNITY_EDITOR
        public SerializableDictionary(Dictionary<K, V> data)
        {
            foreach (var item in data)
            {
                Add(item.Key, item.Value);
            }
        }
#endif
        protected SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
        [SerializeField] private List<CustomKeyValuePair<K, V>> datas = new List<CustomKeyValuePair<K, V>>();
        public void OnBeforeSerialize()
        {
            datas.Clear();

            foreach (KeyValuePair<K, V> pair in this)
            {
                datas.Add(new CustomKeyValuePair<K, V>(pair));
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();
            for (int i = 0, icount = datas.Count; i < icount; ++i)
            {
                this.Add(datas[i].Key, datas[i].Value);
            }
        }
    }
}