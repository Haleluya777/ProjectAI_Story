using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoreanTyper;
using System.Linq;
using TMPro;
using UnityEngine.Playables;
using Unity.VisualScripting;
using System.Text;

public class NewDialogueRunner : MonoBehaviour, DataInitializable
{
    public enum RunnerState { Normal, Skip, Auto }

    [Header("Runner State")]
    [SerializeField] private RunnerState currentState;


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
    [SerializeField] public TextAsset DialogueFile;

    [Header("DialogueParse")]
    [SerializeField] private NewDialogueParser parser;

    [Header("DialogueMenu")]
    [SerializeField] private bool autoTrigger; //대화가 자동으로 진행될지 체크하는 트리거
    [SerializeField] private bool skipTrigger; //대화가 스킵 모드로 진행될지 체크하는 트리거

    [SerializeField] private float settedDialogueTextSpeed; //설정에서 변경된 Dialogue텍스트 진행 속도.
    [SerializeField] private float settedAutoProccessTime; //설정에서 변경된 자동 대화 진행 속도.

    [Header("DialogueCharacters")] //대화에 등장하는 캐릭터 관련 요소들.
    [SerializeField] private GameObject CharacterPrefab; //캐릭터 베이스 프리팹
    [SerializeField] private Transform characterParent; //캐릭터 베이스 프리팹의 부모 오브젝트. Instantiate용.
    [SerializeField] public Dictionary<int, GameObject> characters = new Dictionary<int, GameObject>(); //대화에 등장하는 캐릭터 오브젝트들.

    private const float DIALOGUE_TEXT_SPEED_SKIP = .01f; //텍스트 진행 속도 (스킵 모드)
    private const float DIALOGUE_TEXT_AUTOPROCCESS_SKIP = .01f; //자동 텍스트 넘김 지연 시간. (빠른 모드)

    private WaitForSeconds currentWaitDialogueProccessSpeed; //현재 Dialogue 진행 속도에 쓰는 WaitForSeconds
    private WaitForSeconds currentWaitDialogueAutoProccess; //현재 Dialogue 자동 진행에 쓰는 WaitForSeconds

    private WaitForSeconds skipedWaitDialogueProccessSpeed; //스킵 모드일 때, Dialogue 텍스트 진행 속도에 쓰는 WaitForSeconds
    private WaitForSeconds skipedWaitDialogueAutoProccess; //스킵 모드일 떄, Dialogue 자동 진행에 쓰는 WaitForSeconds

    private WaitForSeconds settedWaitDialogueAutoProccess; //스킵 모드가 아닐 때, Dialogue 자동 진행에 쓰는 WaitForSeconds (설정된 텍스트 속도로 지정함.)
    private WaitForSeconds settedWaitDialogueProccessSpeed; //스킵 모드가 아닐 때, Dialogue 텍스트 진행에 쓰는 WaitForSeconds (설정된 텍스트 속도로 지정.)

    private List<NewDialogueParser.ParsedLine> scriptLine;
    private int currentLineNum = 0;
    [SerializeField] private bool isWaiting;

    private void Start()
    {
        parser.Parse(DialogueFile.text);
        //임시
        CharacterInit(3);
        RunDialogue();
    }

    public void InitializeData()
    {
        currentState = RunnerState.Normal;
        isWaiting = false;

        settedDialogueTextSpeed = .065f;
        settedAutoProccessTime = .6f;

        currentWaitDialogueProccessSpeed = new WaitForSeconds(settedDialogueTextSpeed);
        currentWaitDialogueAutoProccess = new WaitForSeconds(settedAutoProccessTime);

        skipedWaitDialogueAutoProccess = new WaitForSeconds(DIALOGUE_TEXT_AUTOPROCCESS_SKIP);
        skipedWaitDialogueProccessSpeed = new WaitForSeconds(DIALOGUE_TEXT_SPEED_SKIP);

        settedWaitDialogueAutoProccess = new WaitForSeconds(settedAutoProccessTime);
        settedWaitDialogueProccessSpeed = new WaitForSeconds(settedDialogueTextSpeed);
    }

    void Update()
    {
        DialogueStateAction();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentState = RunnerState.Skip;

            ProccessNextLine();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            currentState = RunnerState.Normal;
        }

        if (isWaiting) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (currentLineNum != 0)
            {
                ProccessNextLine();
            }
        }
    }

    //출력 모드에 따른 텍스트 속도 변경.
    private void DialogueStateAction()
    {
        switch (currentState)
        {
            case RunnerState.Normal:
                currentWaitDialogueProccessSpeed = settedWaitDialogueProccessSpeed;
                break;

            case RunnerState.Auto:
                currentWaitDialogueProccessSpeed = settedWaitDialogueProccessSpeed;
                currentWaitDialogueAutoProccess = settedWaitDialogueAutoProccess;
                break;

            case RunnerState.Skip:
                currentWaitDialogueProccessSpeed = skipedWaitDialogueProccessSpeed;
                currentWaitDialogueAutoProccess = skipedWaitDialogueAutoProccess;
                break;
        }
    }

    //설정 메뉴에서 변경된 텍스트 속도에 따른 변수 변경.
    public void SettingChangeTextSpeed(float value)
    {
        settedDialogueTextSpeed = value;
        settedWaitDialogueProccessSpeed = new WaitForSeconds(settedDialogueTextSpeed);
    }

    //설정 메뉴에서 변경된 자동 대화 진행 속도에 따른 변수 변경.
    public void SettingAutoNextLineSpeed(float value)
    {
        settedAutoProccessTime = value;
        settedWaitDialogueAutoProccess = new WaitForSeconds(settedAutoProccessTime);
    }

    public void CharacterInit(int index) //대화에 등장하는 모든 캐릭터 오브젝트 초기화.
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
                CharacterData data = GameManager.instance.dataManager.characterMap.GetCharacter(i);

                character.GetComponent<Image>().sprite = data.characterSpriteMap.sprites["Default"]; //기본 표정으로 초기화.
                character.name = data.characterName;

                //character.SetActive(false);
                character.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -150, 0); //y값 초기화.
                GameManager.instance.dataManager.runningCharacters.Add(data.id, new SystemDataManager.Data { obj = character, characterData = data });
            }
        }
    }
    //타임라인 시그널에 쓰일 이벤트--------------------------
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
        if ((currentState == RunnerState.Auto || currentState == RunnerState.Skip) && !isWaiting)
        {
            ProccessNextLine();
        }
    }

    public void RunDialogue()
    {
        Debug.Log(currentLineNum);
        if (DialogueFile != null)
        {
            scriptLine = parser.Parse(DialogueFile.text);
        }

        Panel.SetActive(true);
        ProccessNextLine();
    }

    public void EndDialogue()
    {
        GameManager.instance.dataManager.runningCharacters.Clear();
        currentLineNum = 0;
        currentState = RunnerState.Normal;
        GameManager.instance.dataManager.dialogueLog.Clear();
        Panel.SetActive(false);
    }
    //------------------------------------------
    public void ProccessNextLine()
    {
        if (currentLineNum >= scriptLine.Count)
        {
            EndDialogue();
            return;
        }

        if (isWaiting) return;

        NewDialogueParser.ParsedLine line = scriptLine[currentLineNum];
        //Debug.Log(line);

        switch (line.Action)
        {
            case "T": //액션 노드가 T일 경우, 대사를 출력.
                if (line.Detail.result.Contains("\\n")) line.Detail.result = line.Detail.result.Replace("\\n", "\n"); //대사에 \n이 포함되어 있을 경우, 줄바꿈 처리.
                Debug.Log(int.Parse(line.Actor.Split('_')[0]));
                SpeakerName.text = line.Actor.Split('_')[1]; //화자 이름 설정.
                CharacterEmphasis(int.Parse(line.Actor.Split('_')[0]), line.Face); //화자 캐릭터 강조.
                StartCoroutine(TypingTxt(line.Detail.result)); //대사 출력.
                GameManager.instance.dataManager.NewdialogueLog.Add(line); //대화 로그에 저장.
                break;

            case "If": //액션 노드가 If일 경우, 조건 체크 및 조건에 부합한지 부합하지 않은지 체크한 후, 줄 이동.
                CheckingCondition(line.Detail.condition.Split('|'), line.Detail.result.Split('|'));
                break;

            case "F": //분기점 체크. 개발 보류.
                //CheckingFlag(line.Detail.condition, line.Detail.result.Split('|'));
                currentLineNum++;
                ProccessNextLine();
                break;

            case "S": //액션 노드가 S일 경우, 선택지를 제시한 후, 고른 선택지에 따라 줄 이동.
                HandleChoices(line.Detail.condition.Split('|'), line.Detail.result.Split('|'), line);
                break;

            default:
                Debug.LogWarning($"알 수 없는 액션: {line.Action} (라인 {currentLineNum})");
                currentLineNum++;
                ProccessNextLine();
                return;
        }
        RunningOtherNode(line);
    }

    private void RunningDialogue()
    {

    }

    private void RunningOtherNode(NewDialogueParser.ParsedLine line)
    {
        if (line.BG != "") GameManager.instance.dialogueFunc.ChangeBG(line.BG);
        if (line.Production != "") GameManager.instance.dialogueFunc.RunningProduction(line.Production);
        //if (line.Face != "") GameManager.instance.dialogueFunc.ChangeFace(line.Face);
        //if (line.Actor != "") GameManager.instance.dialogueFunc.RunningActor(line.Actor);
        if (line.BGM != "") GameManager.instance.dialogueFunc.ChangeBGM(line.BGM);
        if (line.Affection != "") GameManager.instance.dialogueFunc.AffectionChange(line.Affection, line.Actor);
    }

    private void CharacterEmphasis(int id, string emotion) //화자 캐릭터의 강조 및 해당 캐릭터 스프라이트 변경(필요시).
    {
        foreach (Transform character in characterParent)
        {
            character.GetComponent<Image>().color = new Color32(140, 140, 140, 255);
        }
        var emphasisChar = GameManager.instance.dataManager.runningCharacters[id].obj.GetComponent<Image>();
        emphasisChar.color = new Color32(255, 255, 255, 255);
        emphasisChar.sprite = GameManager.instance.dataManager.runningCharacters[id].characterData.characterSpriteMap.sprites[emotion];
    }

    private void CheckingFlag(string condition, string[] results) //분기점 플래그 체크.
    {
        StringBuilder letter = new StringBuilder();
        StringBuilder digit = new StringBuilder();
        condition = condition.Trim();

        while (true)
        {
            if (condition.Contains("||")) //여러 개의 플래그가 ||로 제어될 때.
            {
                Debug.Log("Or연산자 사용.");
            }
            else if (condition.Contains("&&")) //여러 개의 플래그가 &&로 묶일 때.
            {
                Debug.Log("And연산자 사용.");
            }
            else //플래그가 하나 뿐일 때.
            {

            }
        }

    }

    private void CheckingCondition(string[] args, string[] results) //조건 검사 및 참/거짓에 따른 분기 처리.
    {
        for (int i = 0; i < args.Length; i++)
        {
            args[i] = args[i].Trim();
        }
        Debug.Log($"{args[0]}, {args[1]}, {args[2]}");
        if (GameManager.instance.dataManager.operandDic[args[0]] == null) return;
        var leftOperand = GameManager.instance.dataManager.operandDic[args[0]]; //좌측 피 연산자 (데이터 매니저에서 가져옴.)
        var Operator = args[1]; //연산자
        var rightOperand = int.Parse(args[2]); //우측 피 연산자 (int값만 받음.)
        bool condition = false;

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
            Debug.Log("조건 충족");
            currentLineNum += int.Parse(results[0].Trim()) + 1; //조건이 참일 경우, result값(점프할 라인 수)만큼 건너뛰고 다음 줄부터 실행.
            ProccessNextLine();
        }

        else
        {
            Debug.Log("조건 불충족");
            currentLineNum += int.Parse(results[1].Trim()) + 1; //조건이 거짓일 경우, result값(점프할 라인 수)만큼 건너뛰고 다음 줄부터 실행. 
            ProccessNextLine();
        }
    }

    private void HandleChoices(string[] selectors, string[] results, NewDialogueParser.ParsedLine line)
    {
        for (int i = 0; i < selectors.Length; i++)
        {
            selectors[i] = selectors[i].Trim();
            results[i] = results[i].Trim();
        }

        isWaiting = true;
        //ChoiceOptionPanel.SetActive(true);
        DialoguePanel.SetActive(false);

        //foreach (Transform child in OptionContainer)
        //{
        //    Destroy(child.gameObject);
        //}

        for (int i = 0; i < selectors.Length; i++)
        {
            var buttonObj = Instantiate(ChoiceButtonPrefab, OptionContainer);

            var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            var button = buttonObj.GetComponent<Button>();
            int index = i;

            buttonText.text = selectors[i];
            button.onClick.AddListener(() => OptionSelected(int.Parse(results[index]), line));
        }
    }

    private void OptionSelected(int lineIndex, NewDialogueParser.ParsedLine line)
    {
        GameManager.instance.dataManager.NewdialogueLog.Add(line);
        isWaiting = false;
        //ChoiceOptionPanel.SetActive(false);
        DialoguePanel.SetActive(true);
        //currentLineNum = lineIndex;

        foreach (Transform child in OptionContainer)
        {
            Destroy(child.gameObject);
        }

        currentLineNum += lineIndex + 1;
        ProccessNextLine();
    }

    public void ExcuteFunc(string[] args) //DialogueFuncManager의 메서드 실행.
    {
        if (args.Length < 1) return;
        string methodName = args[0];

        object[] parameters = args.Skip(1).ToArray();
        //Debug.Log(parameters.Length);

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
        List<NewDialogueParser.ParsedLine> logs = GameManager.instance.dataManager.NewdialogueLog;
        DialogueLogPanel.SetActive(true);

        foreach (Transform child in DialogueLogContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (NewDialogueParser.ParsedLine log in logs)
        {
            //대화 로그 오브젝트를 생성하는 명령어. 추후 오브젝트 풀링으로 변경 예정.
            var logObj = Instantiate(DialogueLogObj, DialogueLogContainer);
            var speakerLogText = logObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var dialogueLogText = logObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            speakerLogText.text = log.Actor.Split('_')[1];
            dialogueLogText.text = log.Detail.result;
        }
    }

    public void SkipDialogue() //대화 스킵 모드로 변경.
    {
        if (currentState == RunnerState.Skip)
        {
            currentState = RunnerState.Normal;
        }
        else
        {
            currentState = RunnerState.Skip;
        }

        if (currentState == RunnerState.Skip && !isWaiting)
        {
            ProccessNextLine();
        }

        //===============================
        //if (skipTrigger) //스킵 상태일 경우 스킵 버튼을 눌면 스킵 상태 해제.
        //{
        //    skipTrigger = false;
        //    autoTrigger = false;
        //    currentautoProccessTime = settedAutoProccessTime;
        //    currentdialogueTextSpeed = settedDialogueTextSpeed;
        //}
        //else if (!skipTrigger) //스킵 상태가 아닐 경우 스킵 버튼을 누르면 스킵 상태로 변경.
        //{
        //    skipTrigger = true;
        //    autoTrigger = true;
        //
        //    currentWaitDialogueAutoProccess = skipedWaitDialogueAutoProccess;
        //    currentWaitDialogueProccessSpeed = skipedWaitDialogueProccessSpeed;
        //}
        //
        //if (autoTrigger && !isWaiting)
        //{
        //    ProccessNextLine();
        //}
    }

    public void SetAutoMode() //자동 모드로 변경.
    {
        if (currentState == RunnerState.Auto)
        {
            currentState = RunnerState.Normal;
        }
        else
        {
            currentState = RunnerState.Auto;
        }


        //=============================
        //if (skipTrigger)
        //{
        //    skipTrigger = false;
        //    autoTrigger = true;
        //}
        //
        //else
        //{
        //    autoTrigger = !autoTrigger;
        //}
        //
        //currentdialogueTextSpeed = settedDialogueTextSpeed;
        //currentautoProccessTime = settedAutoProccessTime;
        //currentWaitDialogueAutoProccess = new WaitForSeconds(currentautoProccessTime);

        if (currentState == RunnerState.Auto && !isWaiting)
        {
            ProccessNextLine();
        }
    }

    //----------------------

    private IEnumerator TypingTxt(string args)
    {
        isWaiting = true;

        for (int i = 0; i < args.GetTypingLength() + 1; i++)
        {
            DialogueText.text = args.Typing(i);
            yield return currentWaitDialogueProccessSpeed;
        }

        yield return null;
        if (GameManager.instance.timeLineManager.timeLine.state != PlayState.Playing) isWaiting = false;
        currentLineNum++;

        if (currentState == RunnerState.Auto || currentState == RunnerState.Skip)
        {
            yield return currentWaitDialogueAutoProccess;
            ProccessNextLine();
        }
    }
}