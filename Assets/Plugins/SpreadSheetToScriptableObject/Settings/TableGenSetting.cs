using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpreadSheetToScriptableObject
{
    public class TableGenSetting : ScriptableObject
    {
        [SerializeField] private String m_nameSpace = "SpreadSheetToScriptableObject";
        [SerializeField] private String m_codeGenPath = "Assets/Plugins/SpreadSheetToScriptableObject/Generated/Scripts";
        [SerializeField] private String m_assetPath = "Assets/Plugins/SpreadSheetToScriptableObject/Generated/Resources";

        public String NameSpace => m_nameSpace;
        public String CodeGenPath => m_codeGenPath;
        public String AssetPath => m_assetPath;
    }
}