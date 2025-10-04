using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hallelujah;

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
    private const int REPAIRCOUNT = 5;
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
        for (int i = 1; i <= GameManager.instance.dataManager.characterMap.characters.Length; i++)
        {
            var characterData = GameManager.instance.dataManager.characterMap.GetCharacter(i);

            //현재는 CurrnetdialogueNum를 2진수로 변경, 값이 1인 최상위 비트 번호에 해당하는 대화 번호를 가져오게끔 할 예정.
            int characterDialogueNum = 0;//BitGeneric.GetTopBit(characterData.CurrentdialogueNum); //캐릭터의 현재 대화 번호를 가져옴. 다른 방식으로 변경 필요.
            int charID = characterData.id;
            var dialogueFile = characterData.dialogueFiles[characterDialogueNum];
            string floor = characterData.CharacterFloor;

            var characterButton = Instantiate(characterButtonPrefab, characterButtonParent);
            var button = characterButton.GetComponent<Button>();
            button.onClick.AddListener(() => CheckingFlags(characterData));
            button.onClick.AddListener(() => StartDialogue(dialogueFile, characterDialogueNum, floor, charID));
            characterButton.transform.GetChild(0).GetComponent<Image>().sprite = characterData.characterSpriteMap.sprites["Default"];
        }
    }

    private void CheckingFlags(CharacterData character) //CurrentDialgoueNum의 최상위 비트 번호 번째의 대화 파일로 변경.
    {
        var dialogueNum = BitGeneric.GetTopBit(character.CurrentdialogueNum);
        if (GameManager.instance.dialogueRunner.DialogueFile == character.dialogueFiles[dialogueNum]) return;
        GameManager.instance.dialogueRunner.DialogueFile = character.dialogueFiles[dialogueNum];
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
            repairButton.transform.GetChild(0).GetComponent<Image>().sprite = equipmentData.sprite;
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
        GameManager.instance.dataManager.characterMap.GetCharacter(data.reactedCharacterID).CurrentdialogueNum |= (1 << data.progress);
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

    public void StartDialogue(TextAsset dialogue, int lineNum, string floor, int id) //선택한 캐릭터와의 대화 상호작용 시작
    {
        if (GameManager.instance.dataManager.proccessDatas.Routine <= 0) return;
        //DataManager에서 대화 스크립트의 중심 캐릭터의 ID값 설정.
        GameManager.instance.dataManager.MainCharacterID = id;

        //플레이어의 표기된 위치를 클릭한 캐릭터의 위치로 재설정. ex(불의 층 캐릭터와 상호작용을 한다면 우측상단 플레이어의 현재 위치는 불의 층.)
        GameManager.instance.dataManager.proccessDatas.PlayerPosition = floor;
        playerPosition.text = GameManager.instance.dataManager.proccessDatas.PlayerPosition;

        ConsumeActionPoint();
        CharacterSelection.SetActive(false);

        GameManager.instance.dialogueRunner.RunDialogue();
    }

    private void ConsumeActionPoint()
    {
        GameManager.instance.dataManager.proccessDatas.Routine--;
        GameManager.instance.dataManager.proccessDatas.CurrentTime = GameManager.instance.dataManager.dailyRoutine.Next();
        actionPointSlider.value = (float)GameManager.instance.dataManager.proccessDatas.Routine / (float)GameManager.instance.dataManager.maxAP;
    }
}
