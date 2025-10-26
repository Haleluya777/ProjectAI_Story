using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Hallelujah;
using System;
using UnityEngine.UI;

public class SystemDataManager : MonoBehaviour, DataInitializable
{
    //게임 시스템 데이터를 관리하는 매니저 스크립트.
    //여러 캐릭터들의 호감도, 현재 턴, 플레이어가 행동할 수 있는 ActionPoint등, 게임 내에 존재하는 데이터를 보관 및 조정.

    //캐릭터 및 진행 사항을 저장하는 구조체.
    public struct CharacterDatas
    {
        public GameObject obj;
        public CharacterData characterData;
    }

    public struct Floor
    {
        public int floor;
        public string detail;
    }

    public struct ProccessDatas
    {
        public int Day;
        public Floor PlayerPosition;
        public int Routine;
        public string CurrentTime;
    }

    private enum CurrentDialogueCharacter { Iron, Fire, Ground, Water, Wood }
    public int maxAP;
    [SerializeField] public CharacterMap characterMap; //캐릭터 데이터 베이스
    [SerializeField] public RepairableEquipment equipmentMap; //수리 가능한 장비 데이터 베이스
    public FixedDialoguesMap fixedDialoguesMap;
    public Dictionary<int, CharacterDatas> runningCharacters = new Dictionary<int, CharacterDatas>(); //현재 대화에 참여중인 캐릭터들만
    public Dictionary<int, EquipmentDatas> repairableEquipment = new Dictionary<int, EquipmentDatas>(); //수리 장치 데이터
    public SerializedDic_BG backgroundMap; //배경 이미지 데이터 베이스
    public Dictionary<string, object> operandDic = new Dictionary<string, object>(); //조건 체크할 때 쓰는 피연산자.
    public List<NewDialogueParser.ParsedLine> dialogueLog = new List<NewDialogueParser.ParsedLine>(); //지나간 대화 로그.
    public ProccessDatas proccessDatas = new ProccessDatas(); //현재 날짜, 플레이어 위치, 시간, 남은 ap
    public List<string> dayOneTime = new List<string> { "새벽", "오전", "오후_1", "오후_2", "저녁", "휴식 시간" };
    public List<string> currentTime = new List<string> { "아침", "오전 일과", "오후", "오후 일과", "저녁", "밤 일과", "휴식 시간" };
    public CirclularList<string> dailyRoutine { get; private set; }
    [SerializeField] private int[] fixedConversationList = { 31, 21, 21, 21 }; //6개의 비트(하루 루틴 6개)중, 어느 부분에 고정 대화를 실행할 지 체크(1이면 고정대화 존재. 0이면 없음(행동 가능))
    public int floorUnlock; //해금한 층 정보.
    public int repairUnlock; //장치 수리 해금 정보.
    //public List<int> characterDialogueNum = new List<int> { 0, 0, 0, 0, 0 }; //캐릭터의 대화 진행 상황. 2진수로 사용할 예정.
    public int MainCharacterID; //대화의 주체가 되는 중심 캐릭터 ID값. 대화 진행 중, 혹은 대화 마지막에 대화 스크립트 변경 시 CharacterMap에서 해당 캐릭터의 변수값을 변경할 접근용으로 사용. (캐릭터가 메인 로비에 있을 경우 값은 0)

    public void InitializeData()
    {
        characterMap.InitDialogue();
        dailyRoutine = new CirclularList<string>(dayOneTime);
        proccessDatas.PlayerPosition.detail = "1층 | 엘리베이터";
        proccessDatas.CurrentTime = dailyRoutine.Get();
        proccessDatas.Routine = dailyRoutine.Length() - 1;
        proccessDatas.Day = 1;
        floorUnlock = 2;
        repairUnlock = 2;

        CheckingFixedDialogue(proccessDatas.Day, dailyRoutine.IndexOf(dailyRoutine.Get()));
        // for (int i = 0; i < 5; i++)
        // {
        //     Debug.Log(fixedDialoguesMap.GetDialogues(proccessDatas.Day - 1, i).name);
        // }
    }

    public void CheckingFixedDialogue(int day, int time)
    {
        int mask = 1 << time;
        int result = fixedConversationList[day - 1] & mask;
        if (result >> time != 0)
        {
            Debug.Log("고정 대화 있음.");
            GameManager.instance.uiManager.FadeDialogueStart(FxiedDialogueRun);
        }
        else
        {
            Debug.Log("고정 대화 없음. 플레이어 행동 가능");
        }
    }

    private void FxiedDialogueRun()
    {
        //Debug.Log("고정 대화 실행!");
        Debug.Log(fixedDialoguesMap.GetDialogues(proccessDatas.Day - 1, dailyRoutine.IndexOf(dailyRoutine.Get())).name);
        Debug.Log(dailyRoutine.IndexOf(dailyRoutine.Get()));
        GameManager.instance.dialogueRunner.DialogueFile = fixedDialoguesMap.GetDialogues(proccessDatas.Day - 1, dailyRoutine.IndexOf(dailyRoutine.Get()));
        GameManager.instance.dialogueRunner.RunDialogue();
    }

    void Update()
    {
        //Debug.Log(dailyRoutine.Get());
        //Debug.Log(fixedConversationList[proccessDatas.Day - 1]);
        //Debug.Log(proccessDatas.Routine);
        //Debug.Log($"날 : {proccessDatas.Day}, 현재 위치 : {proccessDatas.PlayerPosition}, 현재 시각 : {proccessDatas.Routine}");
        //디버깅용 테스트코드
        //Debug.Log(runningCharacters.ContainsKey(1));
    }

    void Awake()
    {
        //characterDic.Add(1, new CharacterData { name = "Fire", affection = 0 });
    }

    public void ChangeRoutineTime()
    {
        if (proccessDatas.Day > 2) return;
        else dailyRoutine = new CirclularList<string>(currentTime);
    }

    public void AddTurn() //날짜 증가.
    {
        ChangeRoutineTime();
        proccessDatas.Routine = dailyRoutine.Length() - 1;
        proccessDatas.Day++;
        proccessDatas.CurrentTime = dailyRoutine.First(); //회전 리스트의 첫 번째 부분으로 강제 이동.
    }

    public void ConsumeActionPoint()
    {
        proccessDatas.Routine--;
        //Debug.Log(proccessDatas.Routine);
        dailyRoutine.Next();
        proccessDatas.CurrentTime = dailyRoutine.Get();
    }

    public void UnlockFloor(int floor) //해당 번호까지의 모든 층을 해금
    {
        floorUnlock = floor;
    }

    public void UnlockRepair(int num) //해당 번호까지의 모든 시설을 해금
    {
        repairUnlock = num;
    }
}
