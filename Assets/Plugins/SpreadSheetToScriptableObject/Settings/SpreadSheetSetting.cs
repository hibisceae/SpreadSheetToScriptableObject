using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpreadSheetToScriptableObject
{
    [CreateAssetMenu(fileName = "SpreadSheetSetting", menuName = "SpreadSheetToScriptableObject/SpreadSheetSetting")]
    public class SpreadSheetSetting : TableGenSetting
    {
        [SerializeField] private String m_sheetId;
        [SerializeField] private String m_sheetAPIKey;
        [SerializeField] private List<String> m_sheetNames;

        public String SheetId => m_sheetId;
        public String SheetAPIKey => m_sheetAPIKey;
        public List<String> SheetNames => m_sheetNames;
    }

}