using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hallelujah;

public class FloorSelector : MonoBehaviour
{
    [SerializeField] private Transform floorButtonParent;

    public void InitFloorSelection()
    {
        for (int i = 1; i <= GameManager.instance.dataManager.characterMap.characters.Length - 1; i++)
        {
            var characterButton = floorButtonParent.transform.GetChild(i - 1).gameObject;
            var button = characterButton.GetComponent<Button>();

            var characterData = GameManager.instance.dataManager.characterMap.GetCharacter(i);
            int characterDialogueNum = 0;

            int charID = characterData.id;
            var dialogueFile = characterData.dialogueFiles[characterDialogueNum];
            string floor = characterData.floor.floorDetail;

            button.onClick.AddListener(() => CheckingFlags(characterData));
            button.onClick.AddListener(() => InputMainCharacterData(characterData));
            button.onClick.AddListener(() => CheckingNStartDialogue()); //GameManager.instance.uiManager.FadeDialogueStart(StartDialogue));

            button.interactable = false;
        }
        UpdateFloorSelector();
    }

    private void CheckingNStartDialogue()
    {
        if (GameManager.instance.dataManager.proccessDatas.Routine <= 0) return;
        else GameManager.instance.uiManager.FadeDialogueStart(StartDialogue);
    }

    private void InputMainCharacterData(CharacterData data)
    {
        if (GameManager.instance.dataManager.proccessDatas.Routine <= 0) return;
        GameManager.instance.dataManager.MainCharacterData = data;
    }

    private void CheckingFlags(CharacterData character) //CurrentDialgoueNum의 최상위 비트 번호 번째의 대화 파일로 변경.
    {
        //character가 null이면 바로 메서드 종료.
        if (character == null || GameManager.instance.dataManager.proccessDatas.Routine <= 0) return;

        //var dialogueNum = BitGeneric.GetTopBit(character.CurrentdialogueNum); //최상위 비트 인덱스 번호를 반환.
        //Debug.Log(dialogueNum);
        //Debug.Log(character.dialogueFiles[character.CurrnetDialogueIndex] == null);
        if (GameManager.instance.dialogueRunner.DialogueFile == character.dialogueFiles[character.CurrnetDialogueIndex]) return;
        GameManager.instance.dialogueRunner.DialogueFile = character.dialogueFiles[character.CurrnetDialogueIndex];
    }

    public void UpdateFloorSelector()
    {
        if (GameManager.instance.dataManager.floorUnlock == 0) return;
        for (int i = 0; i < GameManager.instance.dataManager.floorUnlock; i++)
        {
            floorButtonParent.transform.GetChild(i).GetComponent<Button>().interactable = true;
            floorButtonParent.transform.GetChild(i).transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void StartDialogue() //선택한 캐릭터와의 대화 상호작용 시작
    {
        //if (GameManager.instance.dataManager.proccessDatas.Routine <= 0) return;
        //DataManager에서 대화 스크립트의 중심 캐릭터의 ID값 설정.
        //GameManager.instance.dataManager.MainCharacterID = id;

        //플레이어의 표기된 위치를 클릭한 캐릭터의 위치로 재설정. ex(불의 층 캐릭터와 상호작용을 한다면 우측상단 플레이어의 현재 위치는 불의 층.) 이거 안쓸 예정.
        //GameManager.instance.dataManager.proccessDatas.PlayerPosition.detail = floor;
        //playerPosition.text = GameManager.instance.dataManager.proccessDatas.PlayerPosition.detail;

        //GameManager.instance.dataManager.ConsumeActionPoint();
        //UpdateAPSlider();
        //CharacterSelection.SetActive(false);

        GameManager.instance.dialogueRunner.RunDialogue(0);
        //Actions.SetActive(true);
    }
}
