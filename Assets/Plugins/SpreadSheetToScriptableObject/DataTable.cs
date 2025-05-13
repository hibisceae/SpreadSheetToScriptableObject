using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpreadSheetToScriptableObject
{

    [Serializable]
    public class DataTable<T> : IEnumerable<T> where T : TableRow
    {
        [SerializeField, ReadOnly] private SerializableDictionary<Int32, T> m_rowByID;

        public IEnumerable<Int32> Keys => m_rowByID.Keys;
        public Int32 Count => m_rowByID.Count;

        public Boolean ContainsID(Int32 pID)
        {
            return m_rowByID.ContainsKey(pID);
        }

        public T Get(Int32 pID)
        {
            if(m_rowByID.TryGetValue(pID, out T row))
            {
                return row;
            }

            Debug.LogError($"DataTable<{typeof(T).Name}> does not contains id [{pID}]");
            return null;
        }

        public T this[Int32 pID] => Get(pID);

        public IEnumerator<T> GetEnumerator()
        {
            return m_rowByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}