using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace SpreadSheetToScriptableObject
{
    public class Example : MonoBehaviour
    {
        [SerializeField] private Dropdown m_idSelectDropDown;
        [SerializeField] private Text m_resultText;

        private DataTable<TestTableRow> m_testTable;
        private List<Int32> m_idList;

        private void Awake()
        {
            DataTable<TestTableRow> m_testTable = DataTableLoader<TestTableRow>.GetTable();

            DataTable<TestTableRow> testTable = DataTableLoader<TestTableRow>.GetTable();
            TestTableRow row = testTable[10001];
            Debug.Log(row.ID);
            Debug.Log(row.Attack);
            Debug.Log(row.Deffence);
            Debug.Log(testTable[10001].ID);

            m_idList = m_testTable.Keys.ToList();
            m_idSelectDropDown.ClearOptions();
            m_idSelectDropDown.AddOptions(m_testTable.Keys.Select(id => id.ToString()).ToList());
            m_idSelectDropDown.onValueChanged.AddListener(OnSelectId);
        }

        private void OnSelectId(int arg0)
        {
            TestTableRow row = m_testTable[m_idList[arg0]];
            m_resultText.text = $"{row.ID} : Attack {row.Attack}, Deffence {row.Deffence}, Lore {row.Lore}";
        }
    }
}
