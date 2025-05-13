using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpreadSheetToScriptableObject
{
    public class DataTableAsset<T> : ScriptableObject where T : TableRow
    {
        [SerializeField] protected DataTable<T> m_table;

        public DataTable<T> Table => m_table;
    }
}