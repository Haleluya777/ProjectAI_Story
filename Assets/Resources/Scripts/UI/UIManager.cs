using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIManager : MonoBehaviour
{
    public DialoguePanel dialogueUIManager;
    public LobbyPanel lobbyUIManager;
    [SerializeField] private Image CoverPanel;

    public void FadeDialogueStart(Action method)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(CoverPanel.DOFade(1, .75f)); //화면 어두워짐
        sequence.AppendCallback(() => method.Invoke()); //회화 시작 메서드 실행.
        sequence.Append(CoverPanel.DOFade(0, .75f)); //화면 밝아짐
    }

    public void FadeDailogueEnd(GameObject dialoguePanel)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(CoverPanel.DOFade(1, .75f)); //화면 어두워짐
        dialoguePanel.SetActive(false);
        sequence.Append(CoverPanel.DOFade(0, .75f)); //화면 밝아짐
        GameManager.instance.dataManager.CheckingFixedDialogue(GameManager.instance.dataManager.proccessDatas.Day, 0);//고정 대화가 있는지 체크 후 실행.
    }
}
