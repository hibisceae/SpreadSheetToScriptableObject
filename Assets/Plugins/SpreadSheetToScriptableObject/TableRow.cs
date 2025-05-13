using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpreadSheetToScriptableObject
{
    [Serializable]
    public class TableRow
    {
        [SerializeField] protected Int32 m_ID;

        public Int32 ID => m_ID;
    }
}