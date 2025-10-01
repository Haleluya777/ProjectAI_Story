using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Hallelujah;

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

    public struct ProccessDatas
    {
        public int Day;
        public string PlayerPosition;
        public int Routine;
        public string CurrentTime;
    }

    //public int currentAP;
    public int maxAP;
    [SerializeField] public CharacterMap characterMap; //캐릭터 데이터 베이스
    [SerializeField] public RepairableEquipment equipmentMap; //수리 가능한 장비 데이터 베이스
    public Dictionary<int, CharacterDatas> runningCharacters = new Dictionary<int, CharacterDatas>(); //현재 대화에 참여중인 캐릭터들만
    public Dictionary<int, EquipmentDatas> repairableEquipment = new Dictionary<int, EquipmentDatas>();
    public SerializedDic_BG backgroundMap; //배경 이미지 데이터 베이스
    public Dictionary<string, object> operandDic = new Dictionary<string, object>(); //조건 체크할 때 쓰는 피연산자.
    public List<NewDialogueParser.ParsedLine> NewdialogueLog = new List<NewDialogueParser.ParsedLine>(); //지나간 대화 로그.
    public List<DialogueParser.ParsedLine> dialogueLog = new List<DialogueParser.ParsedLine>();
    public ProccessDatas proccessDatas;
    private List<string> currentTime = new List<string> { "아침", "오전 일과", "오후", "오후 일과", "저녁", "밤 일과", "휴식 시간" };
    public CirclularList<string> dailyRoutine { get; private set; }

    public void InitializeData()
    {
        dailyRoutine = new CirclularList<string>(currentTime);
        proccessDatas.PlayerPosition = "1층 | 엘리베이터";
        proccessDatas.CurrentTime = dailyRoutine.Next();
        proccessDatas.Routine = maxAP;
        proccessDatas.Day = 1;
    }

    void Update()
    {
        //Debug.Log($"날 : {proccessDatas.Day}, 현재 위치 : {proccessDatas.PlayerPosition}, 현재 시각 : {proccessDatas.Routine}");
    }

    void Awake()
    {
        //characterDic.Add(1, new CharacterData { name = "Fire", affection = 0 });
    }

    public void AddTurn()
    {
        proccessDatas.Routine = maxAP;
        proccessDatas.Day++;
        proccessDatas.CurrentTime = dailyRoutine.First(); //회전 리스트의 첫 번째 부분으로 강제 이동.
    }
}
