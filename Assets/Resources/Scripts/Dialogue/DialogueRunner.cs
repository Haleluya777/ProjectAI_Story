using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoreanTyper;
using System.Linq;
using TMPro;
using UnityEngine.Playables;
using Unity.Profiling;
using Unity.VisualScripting;

public class DialogueRunner : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject Panel;
    [SerializeField] private TextMeshProUGUI SpeakerName;
    [SerializeField] private TextMeshProUGUI DialogueText;
    [SerializeField] private Image NextImg;
    //[SerializeField] private GameObject ChoiceOptionPanel;

    //DialogueBox 관련 요소들
    [SerializeField] private GameObject DialoguePanel; //대화창 전체 부모 오브젝트.
    [SerializeField] private GameObject ChoiceButtonPrefab; //선택지 버튼 프리팹
    [SerializeField] private Transform OptionContainer; // ChoiceButton들의 부모 오브젝트

    //DialogueLog 관련 요소들.
    [SerializeField] private GameObject DialogueLogPanel; //대화 로그 창 전체 부모 오브젝트.
    [SerializeField] private GameObject DialogueLogObj; //대화 로그 창 오브젝트.
    [SerializeField] private Transform DialogueLogContainer; //대화 로그 창의 부모 오브젝트.

    [Header("DialogueFile")]
    [SerializeField] private TextAsset DialogueFile;

    [Header("DialogueParse")]
    [SerializeField] private DialogueParser parser;

    [Header("DialogueMenu")]
    [SerializeField] private bool autoTrigger; //대화가 자동으로 진행될지 체크하는 트리거
    [SerializeField] private float autoProccessTime; //이 변수의 시간동안 대기 후 자동으로 대화 진행.
    [SerializeField] private float dialogueTextSpeed; //Dialogue텍스트가 진행되는 속도

    [Header("DialogueCharacters")] //대화에 등장하는 캐릭터 관련 요소들.
    [SerializeField] private GameObject CharacterPrefab;
    [SerializeField] private CharacterMap characterMap;
    [SerializeField] private Transform characterParent; //대화에 등장하는 캐릭터 오브젝트들의 부모 오브젝트.
    [SerializeField] private Dictionary<int, GameObject> characters = new Dictionary<int, GameObject>(); //대화에 등장하는 캐릭터 오브젝트들.

    private const float DIALOGUE_TEXT_SPEED_SKIP = .01f; //텍스트 진행 속도 (스킵 모드)
    private const float DIALOGUE_TEXT_SPEED_NORMAL = .03f; //텍스트 진행 속도 (초기 모드)
    private const float DIALOGUE_TEXT_AUTOPROCCESS_NORMAL = .5f; //자동 텍스트 넘김 지연 시간. (일반 모드)
    private const float DIALOGUE_TEXT_AUTOPROCCESS_SKIP = .01f; //자동 텍스트 넘김 지연 시간. (빠른 모드)

    private WaitForSeconds waitDialogueProccessSpeed; //Dialogue 진행 속도에 쓰는 WaitForSeconds
    private WaitForSeconds waitDialogueAutoProccess; //Dialogue 자동 진행에 쓰는 WaitForSeconds

    private List<DialogueParser.ParsedLine> scriptLine;
    private int currentLineNum = 0;
    [SerializeField] private bool isWaiting;

    private void Start()
    {
        if (DialogueFile != null)
        {
            scriptLine = parser.Parse(DialogueFile.text);
        }
        isWaiting = false;
        dialogueTextSpeed = DIALOGUE_TEXT_SPEED_NORMAL;
        autoProccessTime = DIALOGUE_TEXT_AUTOPROCCESS_NORMAL;
        waitDialogueProccessSpeed = new WaitForSeconds(dialogueTextSpeed);
        waitDialogueAutoProccess = new WaitForSeconds(autoProccessTime);

        RunDialogue();
    }

    void Update()
    {
        if (isWaiting) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (currentLineNum != 0)
            {
                ProccessNextLine();
            }
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            Time.timeScale = 5f;
            autoTrigger = true;
            autoProccessTime = DIALOGUE_TEXT_AUTOPROCCESS_SKIP;
            waitDialogueAutoProccess = new WaitForSeconds(autoProccessTime);

            ProccessNextLine();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Time.timeScale = 1f;
            autoTrigger = false;
            autoProccessTime = DIALOGUE_TEXT_AUTOPROCCESS_NORMAL;
            waitDialogueAutoProccess = new WaitForSeconds(autoProccessTime);
        }
    }

    public void CharacterInit(int index)
    {
        foreach (Transform character in characterParent)
        {
            character.gameObject.SetActive(false);
        }

        for (int i = 0; i < 32; i++)
        {
            if (((index >> i) & 1) == 1)
            {
                GameObject character = Instantiate(CharacterPrefab, characterParent);
                CharacterData data = characterMap.GetCharacter(i);

                character.GetComponent<Image>().sprite = data.characterSprite;
                character.name = data.characterName;

                character.SetActive(false);
                characters.Add(data.id, character);
            }
        }
    }

    public void DialoguePause()
    {
        Debug.Log("대화 일시정지");
        isWaiting = true;
    }

    public void DialogueResume()
    {
        Debug.Log("대화 재개");
        isWaiting = false;
        GameManager.instance.timeLineManager.TimeLinePause();
    }

    private void RunDialogue()
    {
        //Panel.SetActive(true);
        currentLineNum = 0;
        ProccessNextLine();
    }

    private void EndDialogue()
    {
        //Panel.SetActive(false);
    }

    public void ProccessNextLine()
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

            default: //위의 모든 명령어가 아닌 경우에는 캐릭터의 대화로 간주. 대화창에 출력 및 게임 매니저 대화 로그 리스트에 저장.
                if (line.Args[0].Contains("\\n")) line.Args[0] = line.Args[0].Replace("\\n", "\n");
                Debug.Log(line.Args[0]);
                SpeakerName.text = line.Command;
                GameManager.instance.dialogueLog.Add(line);
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
        //ChoiceOptionPanel.SetActive(true);
        DialoguePanel.SetActive(false);
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
                //Debug.Log("선택지 발견! 버튼 생성!");
                var buttonObj = Instantiate(ChoiceButtonPrefab, OptionContainer);

                var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                var button = buttonObj.GetComponent<Button>();

                string optionText = line.Args[0];
                buttonText.text = optionText;

                int targetLine = scanIndex + 1;
                button.onClick.AddListener(() => OptionSelected(targetLine, line));

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

    private void OptionSelected(int lineIndex, DialogueParser.ParsedLine line)
    {
        GameManager.instance.dialogueLog.Add(line);
        isWaiting = false;
        //ChoiceOptionPanel.SetActive(false);
        DialoguePanel.SetActive(true);
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

    //버튼에 할당할 이벤트 집합
    public void GetDialogueLogs() //GameManager에 저장된 이전까지의 대화 로그.
    {
        List<DialogueParser.ParsedLine> logs = GameManager.instance.dialogueLog;
        DialogueLogPanel.SetActive(true);

        foreach (Transform child in DialogueLogContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (DialogueParser.ParsedLine log in logs)
        {
            //대화 로그 오브젝트를 생성하는 명령어. 추후 오브젝트 풀링으로 변경 예정.
            var logObj = Instantiate(DialogueLogObj, DialogueLogContainer);
            var speakerLogText = logObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var dialogueLogText = logObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            speakerLogText.text = log.Command;
            dialogueLogText.text = log.Args[0];
        }
    }

    public void SkipDialogue() //대화 스킵 모드로 변경.
    {
        SetAutoMode(); //자동 모드 켜기
        if (dialogueTextSpeed == DIALOGUE_TEXT_SPEED_NORMAL)
        {
            Time.timeScale = 5f;
            autoProccessTime = DIALOGUE_TEXT_AUTOPROCCESS_SKIP;
        }
        else
        {
            Time.timeScale = 1f;
            autoProccessTime = DIALOGUE_TEXT_AUTOPROCCESS_NORMAL;
        }
        waitDialogueAutoProccess = new WaitForSeconds(autoProccessTime);
    }

    public void SetAutoMode() //자동 모드로 변경.
    {
        autoTrigger = !autoTrigger;
        dialogueTextSpeed = DIALOGUE_TEXT_SPEED_NORMAL;
        autoProccessTime = DIALOGUE_TEXT_AUTOPROCCESS_NORMAL;

        waitDialogueProccessSpeed = new WaitForSeconds(dialogueTextSpeed);
        waitDialogueAutoProccess = new WaitForSeconds(autoProccessTime);
    }

    //----------------------

    private IEnumerator TypingTxt(string args)
    {
        isWaiting = true;

        for (int i = 0; i < args.GetTypingLength() + 1; i++)
        {
            DialogueText.text = args.Typing(i);
            yield return waitDialogueProccessSpeed;
        }

        yield return null;
        if (GameManager.instance.timeLineManager.timeLine.state != PlayState.Playing) isWaiting = false;
        currentLineNum++;

        if (autoTrigger)
        {
            yield return waitDialogueAutoProccess;
            ProccessNextLine();
        }
    }
}