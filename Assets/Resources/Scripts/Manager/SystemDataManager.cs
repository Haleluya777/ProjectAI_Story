using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemDataManager : MonoBehaviour, DataInitializable
{
    //게임 시스템 데이터를 관리하는 매니저 스크립트.
    //여러 캐릭터들의 호감도, 현재 턴, 플레이어가 행동할 수 있는 ActionPoint등, 게임 내에 존재하는 데이터를 보관 및 조정.

    public int currentAP;
    public int currentTurn;
    [SerializeField] private int maxAP;
    public Dictionary<int, CharacterData> characterDic = new Dictionary<int, CharacterData>();
    public Dictionary<string, object> operandDic = new Dictionary<string, object>();
    public List<NewDialogueParser.ParsedLine> NewdialogueLog = new List<NewDialogueParser.ParsedLine>();
    public List<DialogueParser.ParsedLine> dialogueLog = new List<DialogueParser.ParsedLine>();
    [SerializeField] public CharacterMap characterMap; //캐릭터 데이터 베이스

    public struct CharacterData
    {
        public string name;
        public int affection;
    }

    public void InitializeData()
    {
        currentAP = maxAP;
        currentTurn = 1;
        operandDic.Add("Level", 8);
    }

    void Awake()
    {
        characterDic.Add(1, new CharacterData { name = "Fire", affection = 0 });
    }

    public void AddTurn()
    {
        currentAP = maxAP;
        currentTurn++;
    }
}
