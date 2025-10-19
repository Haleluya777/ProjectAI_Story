using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hallelujah;
using System.Threading;
using Unity.VisualScripting;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private Image lobbyBG;
    [SerializeField] private GameObject CharacterSelection;
    [SerializeField] private GameObject RepairSelection;
    [SerializeField] private GameObject Actions;
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
        InitFloorSelection();
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

    //층 선택 창 초기화.
    private void InitFloorSelection()
    {
        for (int i = 1; i <= GameManager.instance.dataManager.characterMap.characters.Length - 1; i++)
        {
            var characterButton = Instantiate(characterButtonPrefab, characterButtonParent);
            var button = characterButton.transform.GetChild(1).GetComponent<Button>();
            var floorText = characterButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            var characterData = GameManager.instance.dataManager.characterMap.GetCharacter(i);
            int characterDialogueNum = 0;

            int charID = characterData.id;
            var dialogueFile = characterData.dialogueFiles[characterDialogueNum];
            string floor = characterData.floor.floorDetail;

            floorText.text = (i + 1).ToString() + "층";
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = characterData.floor.floorDetail;

            button.onClick.AddListener(() => CheckingFlags(characterData));
            button.onClick.AddListener(() => StartDialogue(dialogueFile, characterDialogueNum, floor, charID));

            button.interactable = false;
        }
        UpdateFloorSelector();
    }

    public void UpdateFloorSelector()
    {
        if (GameManager.instance.dataManager.floorUnlock == 0) return;
        for (int i = 0; i < GameManager.instance.dataManager.floorUnlock; i++)
        {
            characterButtonParent.transform.GetChild(i).transform.GetChild(1).GetComponent<Button>().interactable = true;
        }
    }

    public void UpdateRepairSelector()
    {
        if (GameManager.instance.dataManager.repairUnlock == 0) return;
        for (int i = 0; i < GameManager.instance.dataManager.repairUnlock; i++)
        {
            repairButtonParent.transform.GetChild(i).GetComponent<Button>().interactable = true;
        }
    }

    private void CheckingFlags(CharacterData character) //CurrentDialgoueNum의 최상위 비트 번호 번째의 대화 파일로 변경.
    {
        //character가 null이면 바로 메서드 종료.
        if (character == null) return;

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
            equipmentDic.Add(i, equipment.GetEquipment(i));

            var repairButton = Instantiate(repairButtonPrefab, repairButtonParent);
            var button = repairButton.GetComponent<Button>();
            var equipmentData = equipmentDic[i];

            repairButton.transform.GetChild(0).GetComponent<Image>().sprite = equipmentData.sprite;

            button.onClick.AddListener(() => Repair(equipmentData));
            button.interactable = false;

            repairButton.name = equipmentData.name;
        }
        UpdateRepairSelector();
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
        GameManager.instance.dataManager.ConsumeActionPoint();
        UpdateAPSlider();
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
        //GameManager.instance.dataManager.MainCharacterID = id;

        //플레이어의 표기된 위치를 클릭한 캐릭터의 위치로 재설정. ex(불의 층 캐릭터와 상호작용을 한다면 우측상단 플레이어의 현재 위치는 불의 층.)
        GameManager.instance.dataManager.proccessDatas.PlayerPosition.detail = floor;
        playerPosition.text = GameManager.instance.dataManager.proccessDatas.PlayerPosition.detail;

        GameManager.instance.dataManager.ConsumeActionPoint();
        UpdateAPSlider();
        CharacterSelection.SetActive(false);

        GameManager.instance.dialogueRunner.RunDialogue();
        Actions.SetActive(true);
    }

    private void UpdateAPSlider()
    {
        actionPointSlider.value = (float)GameManager.instance.dataManager.proccessDatas.Routine / (float)GameManager.instance.dataManager.maxAP;
    }
}
