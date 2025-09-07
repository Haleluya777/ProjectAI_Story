using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoreanTyper;
using System.Linq;

public class DialogueRunner : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject Panel;
    [SerializeField] private Text SpeakerName;
    [SerializeField] private Text DialogueText;
    [SerializeField] private Image NextImg;
    [SerializeField] private GameObject ChoiceOptionPanel;
    [SerializeField] private GameObject ChoiceButtonPrefab;
    [SerializeField] private Transform OptionContainer;

    [Header("DialogueFile")]
    [SerializeField] private TextAsset DialogueFile;

    [Header("DialogueParse")]
    [SerializeField] private DialogueParser parser;

    private List<DialogueParser.ParsedLine> scriptLine;
    private int currentLineNum = 0;
    private bool isWaiting = false;

    private void Start()
    {
        if (DialogueFile != null)
        {
            scriptLine = parser.Parse(DialogueFile.text);
        }
    }

    void Update()
    {
        // 테스트용
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (currentLineNum != 0)
            {
                ProccessNextLine();
            }
            else
            {
                RunDialogue();
            }
        }
    }

    private void RunDialogue()
    {
        Panel.SetActive(true);
        currentLineNum = 0;
        ProccessNextLine();
    }

    private void EndDialogue()
    {
        Panel.SetActive(false);
    }

    private void ProccessNextLine()
    {
        if (currentLineNum >= scriptLine.Count)
        {
            EndDialogue();
            return;
        }

        if (isWaiting) return;

        DialogueParser.ParsedLine line = scriptLine[currentLineNum];

        switch (line.Command)
        {
            //아래 4가지 케이스는 아무런 행동 없이 다음 줄로 넘김.
            case "Dialogue":
            case "DialogueEnd":
            case "Result":
            case "IfEnd":
            case "SelectorEnd":
                currentLineNum++;
                ProccessNextLine();
                break;

            case "ResultEnd":
                int selectorEndIndex = FindNextCommand("SelectorEnd", currentLineNum);
                if (selectorEndIndex != -1)
                {
                    currentLineNum = selectorEndIndex + 1; // SelectorEnd 다음 줄로 점프
                }
                else
                {
                    Debug.LogWarning("SelectorEnd not found after ResultEnd!");
                    currentLineNum++; // 못 찾으면 그냥 다음 줄로
                }
                ProccessNextLine();
                break;

            case "Func":
                ExcuteFunc(line.Args);
                currentLineNum++;
                ProccessNextLine();
                break;

            case "Selector":
                currentLineNum++; // Selector 명령어 다음 줄부터 스캔 시작
                HandleChoices();
                return;

            case ">>": // Selector 블록 밖의 >>는 무시하거나 에러 처리 가능
                currentLineNum++;
                ProccessNextLine();
                break;

            case "If":
                CheckingCondition(line.Args);
                break;

            default:
                if (line.Args[0].Contains("\\n")) line.Args[0] = line.Args[0].Replace("\\n", "\n");
                Debug.Log(line.Args[0]);
                SpeakerName.text = line.Command;
                StartCoroutine(TypingTxt(line.Args[0]));
                break;
        }
    }

    private void CheckingCondition(string[] args)
    {
        Debug.Log(args.Length);
        var leftOperand = GameManager.instance.operandDic[args[0]];
        var Operator = args[1];
        bool condition = false;

        object rightOperand = (args[3]) switch
        {
            "Int" => int.Parse(args[2]),
            "Float" => float.Parse(args[2]),
            "Boolean" => bool.Parse(args[2]),
            "String" => args[2].ToString(),
            _ => null
        };

        Debug.Log($"{leftOperand} {Operator} {rightOperand}");

        switch (Operator)
        {
            case "<":
                condition = Convert.ToDouble(leftOperand) < Convert.ToDouble(rightOperand);
                break;

            case ">":
                condition = Convert.ToDouble(leftOperand) > Convert.ToDouble(rightOperand);
                break;

            case "==":
                condition = object.Equals(leftOperand, rightOperand);
                break;

            case "<=":
                condition = Convert.ToDouble(leftOperand) <= Convert.ToDouble(rightOperand);
                break;

            case ">=":
                condition = Convert.ToDouble(leftOperand) >= Convert.ToDouble(rightOperand);
                break;

            case "!=":
                condition = !object.Equals(leftOperand, rightOperand);
                break;
        }

        if (condition)
        {
            currentLineNum++;
            ProccessNextLine();
        }

        else
        {
            int scanIndex = currentLineNum;
            while (scanIndex < scriptLine.Count)
            {
                var line = scriptLine[scanIndex];
                if (line.Command == "IfEnd")
                {
                    currentLineNum = scanIndex;
                    ProccessNextLine();
                    break;
                }
                else scanIndex++;
            }
        }
    }

    private void HandleChoices()
    {
        isWaiting = true;
        ChoiceOptionPanel.SetActive(true);
        int buttonCount = 0;

        foreach (Transform child in OptionContainer)
        {
            Destroy(child.gameObject);
        }

        int scanIndex = currentLineNum;
        while (scanIndex < scriptLine.Count)
        {
            var line = scriptLine[scanIndex];

            if (line.Command == "SelectorEnd") break; // 블록 끝이면 스캔 중지

            if (line.Command == ">>")
            {
                Debug.Log("선택지 발견! 버튼 생성!");
                var buttonObj = Instantiate(ChoiceButtonPrefab, OptionContainer);
                buttonObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-30, 0 + (-100 * buttonCount));

                var buttonText = buttonObj.GetComponentInChildren<Text>();
                var button = buttonObj.GetComponent<Button>();

                string optionText = line.Args[0];
                buttonText.text = optionText;

                int targetLine = scanIndex + 1;
                button.onClick.AddListener(() => OptionSelected(targetLine));

                scanIndex = FindEndOfResultBlock(scanIndex);
                buttonCount++;
            }
            else
            {
                scanIndex++;
            }
        }
    }

    private int FindEndOfResultBlock(int startIndex)
    {
        for (int i = startIndex + 1; i < scriptLine.Count; i++)
        {
            if (scriptLine[i].Command == "ResultEnd")
            {
                return i + 1;
            }
        }
        return scriptLine.Count;
    }

    private int FindNextCommand(string command, int startIndex)
    {
        for (int i = startIndex; i < scriptLine.Count; i++)
        {
            if (scriptLine[i].Command == command)
            {
                return i;
            }
        }
        return -1;
    }

    private void OptionSelected(int lineIndex)
    {
        isWaiting = false;
        ChoiceOptionPanel.SetActive(false);
        currentLineNum = lineIndex;

        foreach (Transform child in OptionContainer)
        {
            Destroy(child.gameObject);
        }

        currentLineNum++;
        ProccessNextLine();
    }

    public void ExcuteFunc(string[] args)
    {
        if (args.Length < 1) return;
        string methodName = args[0];

        object[] parameters = args.Skip(1).ToArray();
        Debug.Log(parameters.Length);

        if (parameters.Length == 0)
        {
            if (GameManager.instance.dialogueFunc.noParamMethod.TryGetValue(methodName, out var action))
            {
                action.Invoke();
            }
        }
        else
        {
            if (GameManager.instance.dialogueFunc.multiParamMethod.TryGetValue(methodName, out var action))
            {
                action.Invoke(parameters);
            }
        }
    }

    private IEnumerator TypingTxt(string args)
    {
        isWaiting = true;
        //yield return new WaitForSeconds(.5f);

        for (int i = 0; i < args.GetTypingLength() + 1; i++)
        {
            DialogueText.text = args.Typing(i);
            yield return new WaitForSeconds(.03f);
        }

        yield return null;
        isWaiting = false;
        currentLineNum++;
    }
}