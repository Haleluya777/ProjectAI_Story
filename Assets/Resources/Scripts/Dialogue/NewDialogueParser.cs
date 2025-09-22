using System.Collections.Generic;
using UnityEngine;

public class NewDialogueParser : MonoBehaviour
{
    public struct ParsedLine
    {
        public string Action;
        public string[] Detail;
        public string BG;
        public string Production;
        public string Face;
        public string Actor;
        public string BGM;
        public string Affection;
        public int LineNum;
    }

    public List<ParsedLine> Parse(string csvFile)
    {
        List<ParsedLine> parsedLines = new List<ParsedLine>();
        string[] lines = csvFile.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            List<string> parts = new List<string>(line.Split(','));
            parts.RemoveAll(s => string.IsNullOrEmpty(s));

            for (int j = 0; j < parts.Count; j++)
            {
                parts[j] = parts[j].Trim();
            }

            string action = parts[0];
            List<string> detail = new List<string>();
        }

        return parsedLines;
    }
}
