using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private GameObject CharacterSelection;
    [SerializeField] private Button playerActionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private GameObject characterButtonPrefab;
    [SerializeField] private Transform characterButtonParent;

    void Start()
    {
        //playerActionButton.onClick.AddListener(StartAction);
        InitCharacterSelection();
    }

    private void InitCharacterSelection()
    {
        for (int i = 0; i < GameManager.instance.dataManager.characterMap.characters.Length; i++)
        {
            int characterDialogueNum = GameManager.instance.dataManager.characterMap.GetCharacter(i).CurrentdialogueNum;
            var dialogueFile = GameManager.instance.dataManager.characterMap.GetCharacter(i).dialogueFiles[characterDialogueNum];
            Debug.Log(dialogueFile.name);
            var characterButton = Instantiate(characterButtonPrefab, characterButtonParent);
            characterButton.GetComponent<Button>().onClick.AddListener(() => StartDialogue(dialogueFile, characterDialogueNum));
            characterButton.transform.GetChild(0).GetComponent<Image>().sprite = GameManager.instance.dataManager.characterMap.GetCharacter(i).characterSpriteMap.sprites["Default"];
        }
    }

    public void Repair()
    {
        if (GameManager.instance.dataManager.currentAP <= 0) return;
        Debug.Log("수리");
        ConsumeActionPoint();
    }

    public void ProccessNextDay()
    {
        GameManager.instance.dataManager.AddTurn();
        //playerActionButton.onClick.RemoveAllListeners();
        //playerActionButton.onClick.AddListener(StartAction);
    }

    public void SelectCharacter()
    {
        CharacterSelection.SetActive(true);
    }

    public void StartDialogue(TextAsset dialogue, int lineNum) //선택한 캐릭터와의 대화 상호작용 시작
    {
        if (GameManager.instance.dataManager.currentAP <= 0) return;
        //Debug.Log(GameManager.instance.dataManager.currentAP);

        ConsumeActionPoint();
        CharacterSelection.SetActive(false);
        //this.gameObject.SetActive(false);

        //DialogueManager의 DialogueFile을 선택한 캐릭터의 대화 파일로 지정.
        GameManager.instance.dialogueRunner.DialogueFile = dialogue;

        GameManager.instance.dialogueRunner.RunDialogue();
        //if (GameManager.instance.dataManager.currentAP <= 0)
        //{
        //    playerActionButton.onClick.RemoveAllListeners();
        //    playerActionButton.onClick.AddListener(ProccessNextDay);
        //    actionButtonText.text = "다음 날";
        //}
    }

    private void ConsumeActionPoint()
    {
        GameManager.instance.dataManager.currentAP--;
    }
}
