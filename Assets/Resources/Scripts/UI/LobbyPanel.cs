using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private Image lobbyBG;
    [SerializeField] private GameObject CharacterSelection;
    [SerializeField] private GameObject RepairSelection;
    [SerializeField] private Button playerActionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private GameObject characterButtonPrefab;
    [SerializeField] private GameObject repairButtonPrefab;
    [SerializeField] private Transform characterButtonParent;
    [SerializeField] private Transform repairButtonParent;
    [SerializeField] private Slider actionPointSlider;
    [SerializeField] private TextMeshProUGUI playerPosition;
    [SerializeField] private TextMeshProUGUI day;
    private const int REPAIRCOUNT = 3;
    void Start()
    {
        //playerActionButton.onClick.AddListener(StartAction);
        InitCharacterSelection();
        InitRepairSelection();
    }

    private void Update()
    {
        //Debug.Log(GameManager.instance.dataManager.proccessDatas.Routine);
        for (int i = 0; i < REPAIRCOUNT; i++)
        {
            //Debug.Log(GameManager.instance.dataManager.repairableEquipment[i].name + " : " + GameManager.instance.dataManager.repairableEquipment[i].progress);
        }
    }

    private void InitCharacterSelection()
    {
        for (int i = 0; i < GameManager.instance.dataManager.characterMap.characters.Length; i++)
        {
            var characterData = GameManager.instance.dataManager.characterMap.GetCharacter(i);
            int characterDialogueNum = characterData.CurrentdialogueNum;
            var dialogueFile = characterData.dialogueFiles[characterDialogueNum];
            string floor = characterData.CharacterFloor;

            var characterButton = Instantiate(characterButtonPrefab, characterButtonParent);
            characterButton.GetComponent<Button>().onClick.AddListener(() => StartDialogue(dialogueFile, characterDialogueNum, floor));
            characterButton.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.instance.dataManager.characterMap.GetCharacter(i).characterSpriteMap.sprites["Default"];
        }
    }

    private void InitRepairSelection()
    {
        Dictionary<int, EquipmentDatas> equipmentDic = GameManager.instance.dataManager.repairableEquipment;
        var equipment = GameManager.instance.dataManager.equipmentMap;

        for (int i = 0; i < REPAIRCOUNT; i++)
        {
            var repairButton = Instantiate(repairButtonPrefab, repairButtonParent);
            equipmentDic.Add(i, equipment.GetEquipment(i));
            var equipmentData = equipmentDic[i];
            repairButton.GetComponent<Button>().onClick.AddListener(() => Repair(equipmentData));
            repairButton.name = equipmentData.name;
        }
    }

    public void PlayerPosUpdate(string pos)
    {
        playerPosition.text = pos;
    }

    public void Repair(EquipmentDatas data)
    {
        if (GameManager.instance.dataManager.proccessDatas.Routine <= 0) return;
        Debug.Log("수리");
        data.progress++;
        RepairSelection.SetActive(false);
        ConsumeActionPoint();
    }

    public void ProccessNextDay()
    {
        GameManager.instance.dataManager.AddTurn();
        day.text = "Day" + GameManager.instance.dataManager.proccessDatas.Day.ToString();
        actionPointSlider.value = (float)GameManager.instance.dataManager.proccessDatas.Routine / (float)GameManager.instance.dataManager.maxAP;
    }

    public void StartDialogue(TextAsset dialogue, int lineNum, string floor) //선택한 캐릭터와의 대화 상호작용 시작
    {
        if (GameManager.instance.dataManager.proccessDatas.Routine <= 0) return;
        //플레이어의 표기된 위치를 클릭한 캐릭터의 위치로 재설정. ex(불의 층 캐릭터와 상호작용을 한다면 우측상단 플레이어의 현재 위치는 불의 층.)
        GameManager.instance.dataManager.proccessDatas.PlayerPosition = floor;
        playerPosition.text = GameManager.instance.dataManager.proccessDatas.PlayerPosition;

        ConsumeActionPoint();
        CharacterSelection.SetActive(false);

        //DialogueManager의 DialogueFile을 선택한 캐릭터의 대화 파일로 지정.
        GameManager.instance.dialogueRunner.DialogueFile = dialogue;
        GameManager.instance.dialogueRunner.RunDialogue();
    }

    private void ConsumeActionPoint()
    {
        GameManager.instance.dataManager.proccessDatas.Routine--;
        GameManager.instance.dataManager.proccessDatas.CurrentTime = GameManager.instance.dataManager.dailyRoutine.Next();
        actionPointSlider.value = (float)GameManager.instance.dataManager.proccessDatas.Routine / (float)GameManager.instance.dataManager.maxAP;
    }
}
