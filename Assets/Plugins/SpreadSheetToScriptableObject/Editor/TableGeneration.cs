using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using UObject = UnityEngine.Object;
using SObject = System.Object;
namespace SpreadSheetToScriptableObject
{
    public class TableGeneration
    {
        public static void GenerateCode(
            TableGenSetting pSetting,
            ICSVLoader pCSVLoader,
            IEnumerable<String> pSheetNames)
        {
            foreach (String sheetNames in pSheetNames)
            {
                List<List<String>> data = pCSVLoader.LoadCSV(sheetNames);
                List<(String, String)> headers = new List<(string, string)>();

                for (int i = 0; i < data[0].Count; i++)
                    headers.Add((data[0][i], data[1][i]));

                GenerateCode(pSetting, sheetNames, headers);
            }

            AssetDatabase.Refresh();
        }

        private static void GenerateCode(TableGenSetting pSetting, String pSheetName, List<(String, String)> pSheetHeader)
        {
            CodeTypeDeclaration rowDeclaration = new CodeTypeDeclaration($"{pSheetName}Row");
            rowDeclaration.BaseTypes.Add(typeof(TableRow));
            rowDeclaration.IsClass = true;
            rowDeclaration.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(
                        typeof(SerializableAttribute)
                        )
                    )
                );

            foreach (var header in pSheetHeader)
            {
                if (header.Item1.Equals("ID"))
                    continue;

                Type type = Type.GetType($"System.{header.Item2}");
                if (type == null)
                    type = Type.GetType($"{header.Item2}, Assembly-CSharp");
                if (type == null)
                {
                    Debug.LogError($"Type {header.Item2} not found");
                    continue;
                }

                CodeMemberField field = new CodeMemberField();
                field.Name = $"m_{header.Item1}";
                field.Type = new CodeTypeReference(type);

                field.Attributes = MemberAttributes.Private;
                field.CustomAttributes.Add(
                    new CodeAttributeDeclaration(
                        new CodeTypeReference(
                            typeof(SerializeField)
                            )
                        )
                    );
                rowDeclaration.Members.Add(field);

                CodeMemberProperty property = new CodeMemberProperty();
                property.Name = header.Item1;
                property.Type = new CodeTypeReference(type);
                property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                property.HasGet = true;
                property.HasSet = false;
                property.GetStatements.Add(
                    new CodeMethodReturnStatement(
                        new CodeFieldReferenceExpression(
                            new CodeThisReferenceExpression(),
                            field.Name
                            )
                        )
                    );

                rowDeclaration.Members.Add(property);
            }

            CodeTypeDeclaration assetDeclaration = new CodeTypeDeclaration($"{pSheetName}Asset");
            CodeTypeReference assetTypeReference = new CodeTypeReference(typeof(DataTableAsset<>));
            assetTypeReference.TypeArguments.Add(new CodeTypeReference($"{pSetting.NameSpace}.{pSheetName}Row"));
            assetDeclaration.BaseTypes.Add(assetTypeReference);
            assetDeclaration.IsClass = true;

            CodeNamespace codeNamespace = new CodeNamespace(pSetting.NameSpace);

            codeNamespace.Types.Add(assetDeclaration);
            codeNamespace.Types.Add(rowDeclaration);

            CodeCompileUnit compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(codeNamespace);

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            if (!Directory.Exists(pSetting.CodeGenPath))
                Directory.CreateDirectory(pSetting.CodeGenPath);

            using (StreamWriter sourceWriter = new StreamWriter(Path.Combine(pSetting.CodeGenPath, $"{pSheetName}.generated.cs")))
            {
                provider.GenerateCodeFromCompileUnit(compileUnit, sourceWriter, new CodeGeneratorOptions());
            }

            Debug.Log($"Generated {pSheetName}.generated.cs");
        }

        public static void SyncData(
            TableGenSetting pSetting,
            ICSVLoader pCSVLoader,
            IEnumerable<String> pSheetNames)
        {
            foreach (String sheetName in pSheetNames)
            {
                List<List<String>> data = pCSVLoader.LoadCSV(sheetName);
                List<(String, String)> headers = new List<(String, String)>();

                for (int i = 0; i < data[0].Count; i++)
                    headers.Add((data[0][i], data[1][i]));

                SyncData(pSetting, sheetName, data, headers);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void SyncData(TableGenSetting pSetting, String pSheetName, List<List<String>> pData, List<(String, String)> pHeaders)
        {
            Type rowType = Type.GetType($"{pSetting.NameSpace}.{pSheetName}Row,Assembly-CSharp");
            Type assetType = Type.GetType($"{pSetting.NameSpace}.{pSheetName}Asset,Assembly-CSharp");
            if (rowType == null || assetType == null)
            {
                Debug.LogError($"Could not find type of {pSheetName}row or {pSheetName}asset. please generate code first.");
                return;
            }

            String path = Path.Combine(pSetting.AssetPath, $"{pSheetName}Asset.asset");
            UObject asset = AssetDatabase.LoadAssetAtPath<UObject>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance(assetType);

                if (!Directory.Exists(pSetting.AssetPath))
                    Directory.CreateDirectory(pSetting.AssetPath);

                AssetDatabase.CreateAsset(asset, path);
            }

            if (pData.Count <= 2)
                return;

            List<(Int32, SObject)> values = new List<(Int32, SObject)>();
            for (Int32 i = 2; i < pData.Count; i++)
            {
                Int32 id = 0;
                SObject row = Activator.CreateInstance(rowType);

                for (Int32 j = 0; j < pData[i].Count; j++)
                {
                    if (pHeaders[j].Item1.Equals("ID"))
                    {
                        id = Int32.Parse(pData[i][j]);
                    }

                    FieldInfo fieldInfo = rowType.GetField($"m_{pHeaders[j].Item1}", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo == null)
                    {
                        Debug.LogError($"could not find Field of m_{pHeaders[j].Item1}");
                        return;
                    }

                    SObject parsedValue = Convert.ChangeType(pData[i][j], fieldInfo.FieldType);
                    fieldInfo.SetValue(row, parsedValue);
                }

                values.Add((id, row));
            }

            FieldInfo tableField = assetType.GetField("m_table", BindingFlags.Instance | BindingFlags.NonPublic);
            SObject table = tableField.GetValue(asset);

            FieldInfo dictionaryField = table.GetType().GetField("m_rowByID", BindingFlags.Instance | BindingFlags.NonPublic);
            SObject dictionary = dictionaryField.GetValue(table);

            MethodInfo clearMethod = dictionary.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            clearMethod.Invoke(dictionary, null);

            MethodInfo addMethod = dictionary.GetType().GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            foreach (var idRowPair in values)
            {
                addMethod.Invoke(dictionary, new SObject[] { idRowPair.Item1, idRowPair.Item2 });
            }

            EditorUtility.SetDirty(asset);
        }
    }

}