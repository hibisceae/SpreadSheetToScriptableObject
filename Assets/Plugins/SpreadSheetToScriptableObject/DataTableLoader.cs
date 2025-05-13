using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpreadSheetToScriptableObject
{
    public static class DataTableLoader<TRow> where TRow : TableRow
    {
        private static DataTable<TRow> m_table;

        public static DataTable<TRow> GetTable()
        {
            if (m_table == null)
            {
                var assets = Resources.LoadAll<DataTableAsset<TRow>>("");
                if (assets.Length == 0)
                {
                    Debug.LogError("No DataTableAsset found in Resources folder.");
                    return null;
                }
                m_table = assets[0].Table;
            }
            return m_table;
        }
    }
}