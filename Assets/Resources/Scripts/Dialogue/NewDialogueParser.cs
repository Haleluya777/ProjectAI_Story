using System.Collections.Generic;
using UnityEngine;

//JSON test
[System.Serializable]
public class DataWarpper
{
    public string Action;
    public string Detail;
    public string BG;
    public string Production;
    public string Face;
    public string Actor;
    public string BGM;
    public string Affection;
}

[System.Serializable]
public class DialogueDatas
{
    public List<DataWarpper> TestedSheet;
}
//---------------------------------------
public class NewDialogueParser : MonoBehaviour
{
    public struct ParsedLine
    {
        public string Action;
        public Detail Detail;
        public string BG;
        public string Production;
        public string Face;
        public string Actor;
        public string BGM;
        public string Affection;
        public int LineNum;
    }

    public struct Detail
    {
        public string condition;
        public string result;
    }

    public List<ParsedLine> Parse(string csvFile)
    {
        //JSon test
        //DialogueDatas testline = JsonUtility.FromJson<DialogueDatas>(csvFile);
        //Debug.Log(testline.TestedSheet.Count);

        List<ParsedLine> parsedLines = new List<ParsedLine>();
        string[] lines = csvFile.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            List<string> parts = new List<string>(line.Split('\t'));
            //parts.RemoveAll(s => string.IsNullOrEmpty(s));

            for (int j = 0; j < parts.Count; j++)
            {
                parts[j] = parts[j].Trim();
                //Debug.Log(parts[j]);
            }

            string action = parts[0]; //첫번째 열(행동)을 action변수에 저장.
            string _condition = "";
            string _result = "";

            if (parts[1].Contains('_'))
            {
                _condition = parts[1].Split('_')[0]; //두번째 열을 '_'로 나누어 앞부분을 _condition변수에 저장.
                _result = parts[1].Split('_')[1]; //두번째 열을 '_'로 나누어 뒷부분을 _result변수에 저장.
            }
            else
            {
                _condition = ""; //두번째 열에 '_'가 없을 경우, 전체를 _condition변수에 저장.
                _result = parts[1]; //_result변수는 빈 문자열로 초기화.
            }

            Detail detail = new Detail
            {
                condition = _condition,
                result = _result
            };

            ParsedLine parsedLine = new ParsedLine
            {
                Action = action,
                Detail = detail,
                BG = parts[2],
                Production = parts[3],
                Face = parts[4],
                Actor = parts[5],
                BGM = parts[6],
                Affection = parts[7],
                LineNum = i
            };

            parsedLines.Add(parsedLine);
        }

        //for (int i = 0; i < parsedLines.Count; i++)
        //{
        //    Debug.Log($"라인 {i} : Action={parsedLines[i].Action}, Condition={parsedLines[i].Detail.condition}, Result={parsedLines[i].Detail.result}, BG={parsedLines[i].BG}, Production={parsedLines[i].Production}, Face={parsedLines[i].Face}, Actor={parsedLines[i].Actor}, BGM={parsedLines[i].BGM}, Affection={parsedLines[i].Affection}");
        //}
        return parsedLines;
    }
}
