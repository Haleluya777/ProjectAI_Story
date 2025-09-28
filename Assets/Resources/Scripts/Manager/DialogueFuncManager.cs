using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;

public class DialogueFuncManager : MonoBehaviour, DataInitializable
{
    public Dictionary<string, Action> noParamMethod = new Dictionary<string, Action>();
    public Dictionary<string, Action<object[]>> multiParamMethod = new Dictionary<string, Action<object[]>>();


    public void InitializeData()
    {
        //파라미터가 없는 메서드들 초기화
        noParamMethod.Add("Greeting", Greeting);

        //파라미터가 존재하는 메서드를 초기화
        multiParamMethod.Add("Greeting2", (param) => Greeting2(param[0].ToString(), int.Parse((string)param[1])));
        multiParamMethod.Add("Add", (param) => Add(int.Parse((string)param[0])));
        multiParamMethod.Add("MultiParamTest", (param) => MultiParamTest(param[0].ToString(), int.Parse((string)param[1])));
        multiParamMethod.Add("TimeLineRun", (param) => TimeLineStart(int.Parse((string)param[0])));
        multiParamMethod.Add("CharacterInit", (param) => CharacterInit(int.Parse((string)param[0])));
        multiParamMethod.Add("TimeLineInsert", (param) => TimeLineInsert(int.Parse((string)param[0])));
    }

    public void TimeLineInsert(int characterId)
    {
        Animator[] anim = GameManager.instance.dialogueRunner.characters.Values.Where(obj => obj.GetComponent<Animator>() != null).Select(obj => obj.GetComponent<Animator>()).ToArray();
        GameManager.instance.timeLineManager.TimeLineAnimatorTrackInsert(anim);
    }

    public void CharacterInit(int index)
    {
        GameManager.instance.dialogueRunner.CharacterInit(index);
    }

    public void TimeLineStart(int timeLineId)
    {
        GameManager.instance.timeLineManager.TimeLinePlay(timeLineId);
    }

    public void Greeting()
    {
        Debug.Log("안녕!");
    }

    public void Greeting2(string greeting, int num)
    {
        Debug.Log($"{greeting}, {num}");
    }

    public void Add(int num)
    {
        Debug.Log(num);
    }

    public void MultiParamTest(string name, int age)
    {
        Debug.Log($"이름은 {name}, 나이는 {age}세다.");
    }


    //===============8개의 노드 중 6개의 노드 진행======================
    public void ChangeBG(string bgNode)
    {
        string[] nodeSplit = bgNode.Split('_');
        int bgNum = int.Parse(nodeSplit[1]);

        //DialoguePanel클래스의 backGround오브젝트의 이미지를 DataSystemManager에서 가져온 스프라이트 값으로 변경.
        GameManager.instance.uiManager.dialogueUIManager.backGround.sprite = GameManager.instance.dataManager.backgroundMap.bgMap[bgNum];
        //
    }

    public void RunningProduction(string production)
    {
        string[] nodeSplit = production.Split('_');
        switch (nodeSplit[1])
        {
            case "Move":

                break;

            case "Turn":
                break;

            case "Fall":
                break;
            default:
                break;
        }
    }

    public void ChangeBGM(string bgm)
    {
        string[] nodeSplit = bgm.Split('_');
        int bgmNum = int.Parse(nodeSplit[1]);

        GameManager.instance.soundManager.PlayBGM(bgmNum);
    }

    public void RunSE(string se)
    {
        string[] nodeSplit = se.Split('_');
        int seNum = int.Parse(nodeSplit[1]);

        GameManager.instance.soundManager.PlaySE(seNum);
    }

    public void AffectionChange(string affection, string actor)
    {
        int actorId = int.Parse(actor.Split('_')[0]);
        int value = int.Parse(affection);
        GameManager.instance.dataManager.runningCharacters[actorId].characterData.affaction += value;
        Debug.Log($"{GameManager.instance.dataManager.runningCharacters[actorId].characterData.characterName}의 호감도가 {value}만큼 변동되었습니다. 현재 호감도 : {GameManager.instance.dataManager.runningCharacters[actorId].characterData.affaction}");
    }
    //===============================================================
}
