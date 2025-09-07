using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public struct ParsedLine
    {
        public string Command;
        public string[] Args;
        public int LineNum;
    }

    public List<ParsedLine> Parse(string csvFile)
    {
        List<ParsedLine> parsedLines = new List<ParsedLine>();
        string[] lines = csvFile.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            //Debug.Log($"라인{line}");
            if (string.IsNullOrWhiteSpace(line)) continue;

            List<string> parts = new List<string>(line.Split(','));
            parts.RemoveAll(s => string.IsNullOrEmpty(s));

            for (int j = 0; j < parts.Count; j++)
            {
                parts[j] = parts[j].Trim();
                //Debug.Log(parts[j]);
            }

            string command = "";
            List<string> args = new List<string>();

            for (int j = 0; j < parts.Count; j++)
            {
                if (!string.IsNullOrEmpty(parts[j]))
                {
                    command = parts[j];
                    args.AddRange(parts.GetRange(j + 1, parts.Count - (j + 1)));
                    break;
                }
            }
            if (!string.IsNullOrEmpty(command))
            {
                parsedLines.Add(new ParsedLine
                {
                    Command = command,
                    Args = args.ToArray(),
                    LineNum = i
                });
            }
        }
        return parsedLines;
    }
}
