using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SpreadSheetToScriptableObject
{
    [CustomEditor(typeof(SpreadSheetSetting))]
    public class SpreadSheetSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SpreadSheetSetting setting = (SpreadSheetSetting)target;

            GUILayout.Label("Generate");
            if (GUILayout.Button("Generate Code"))
            {
                TableGeneration.GenerateCode(
                    setting,
                    new SpreadSheetCSVLoader(
                        setting.SheetAPIKey,
                        setting.SheetId),
                    setting.SheetNames);
            }
            if (GUILayout.Button("Generate/Sync Asset"))
            {
                TableGeneration.SyncData(
                    setting,
                    new SpreadSheetCSVLoader(
                        setting.SheetAPIKey,
                        setting.SheetId),
                    setting.SheetNames);
            }
            EditorGUILayout.Separator();
            GUILayout.Label("Clear");
            if (GUILayout.Button("Clear Generated Code"))
            {
                Directory.Delete(setting.CodeGenPath,true);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Clear Generated Asset"))
            {
                Directory.Delete(setting.AssetPath, true);
                AssetDatabase.Refresh();
            }
        }
    }

}