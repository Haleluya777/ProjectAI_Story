using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemDataManager : MonoBehaviour
{
    //게임 시스템 데이터를 관리하는 매니저 스크립트.
    //여러 캐릭터들의 호감도, 현재 턴, 플레이어가 행동할 수 있는 ActionPoint등, 게임 내에 존재하는 데이터를 보관 및 조정.

    public int currentAP;
    public int currentTurn;
    [SerializeField] private int maxAP;
    public Dictionary<string, object> operandDic = new Dictionary<string, object>();
    public List<DialogueParser.ParsedLine> dialogueLog = new List<DialogueParser.ParsedLine>();

    public void AddTurn()
    {
        currentAP = maxAP;
        currentTurn++;
    }
}
