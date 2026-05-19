using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utilities
{
    public class ReadFromCsv : MonoBehaviour
    {
        /// <summary>
        /// Only works in non-web builds (untested in full builds too lol), so won't be used for now
        /// </summary>
        public static string[] ReadCsvNonWeb(string path)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, path);
            StreamReader reader = new StreamReader(filePath);
            List<string> lines = new List<string>();
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                lines.AddRange(line.Split(','));
            }

            return lines.ToArray();
        }

        public static List<List<string>> ReadCsv(string resourcePath)
        {
            TextAsset csvFile = Resources.Load<TextAsset>(resourcePath);
            if (csvFile == null)
            {
                UnityEngine.Debug.LogError($"CSV file not found at Resources/{resourcePath}");
                return null;
            }

            List<List<string>> data = new List<List<string>>();
            string[] lines = csvFile.text.Split('\n');
            foreach (string line in lines)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(line)) continue;
                List<string> row = new List<string>();
                string[] columns = line.Trim().Split(',');
                foreach (string column in columns)
                {
                    row.Add(column.Trim());
                }

                data.Add(row);
            }

            return data;
        }
    }
}