using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace SpreadSheetToScriptableObject
{
    public class SpreadSheetCSVLoader : ICSVLoader
    {
        private readonly String m_apiKey;
        private readonly String m_sheetID;
        public SpreadSheetCSVLoader(String pAPIKey, String pSheetID)
        {
            m_apiKey = pAPIKey;
            m_sheetID = pSheetID;
        }

        public List<List<string>> LoadCSV(string pPath)
        {
            List<List<String>> result = null;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"https://sheets.googleapis.com/v4/spreadsheets/{m_sheetID}/values/{pPath}?key={m_apiKey}";

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    response.EnsureSuccessStatusCode();

                    String responseBody = response.Content.ReadAsStringAsync().Result;

                    JObject jsonObject = JObject.Parse(responseBody);
                    JArray valuesArray = (JArray)jsonObject["values"];
                    result = valuesArray.Select(
                        subArray => subArray.Select(token => (String)token).ToList()
                    ).ToList();
                }
                catch (HttpRequestException e)
                {
                    Debug.LogError("Error fetching data from Google Sheets: " + e.Message);
                }
            }
            return result;
        }
    }
}