using System;
using System.Collections.Generic;

namespace SpreadSheetToScriptableObject
{
    public interface ICSVLoader
    {
        List<List<String>> LoadCSV(String pPath);
    }
}