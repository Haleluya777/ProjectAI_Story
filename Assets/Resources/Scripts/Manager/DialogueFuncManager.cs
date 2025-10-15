using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Timeline;

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
    }

    public void CharacterInit(int index)
    {
        GameManager.instance.dialogueRunner.CharacterInit(index);
    }

    public void TimeLineStart(int timeLineId)
    {
        //GameManager.instance.timeLineManager.TimeLinePlay(timeLineId);
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
        //각 캐릭터가 사용할 애니메이션 트랙
        Dictionary<int, AnimationTrack[]> characterTrack = new Dictionary<int, AnimationTrack[]>();

        GameManager.instance.timeLineBuilder.InitTimeLine();
        string[] blocks = production.Split(',');

        for (int i = 0; i < blocks.Length; i++)
        {
            string[] nodeSplit = blocks[i].Split('_');
            var actorId = int.Parse(nodeSplit[0]);

            if (!characterTrack.ContainsKey(actorId))
            {
                Debug.Log($"새로운 캐릭터 {actorId}의 트랙 생성.");
                var track = GameManager.instance.timeLineBuilder.TrackSetting(new AnimationTrack[5]);
                characterTrack.Add(actorId, track);
            }

            if (actorId == -1)
            {
                GameManager.instance.timeLineBuilder.SetAnimator(characterTrack[actorId][0], GameManager.instance.uiManager.gameObject);
            }
            else
            {
                GameManager.instance.timeLineBuilder.SetAnimator(characterTrack[actorId][0], GameManager.instance.dataManager.runningCharacters[actorId].obj);
            }

            switch (nodeSplit[1])
            {
                case "Move":
                    {
                        //int actorId = int.Parse(nodeSplit[0]); //타임라인에서 애니메이션을 재생할 캐릭터 ID
                        string[] details = nodeSplit[2].Split('|'); //출발 위치(1), 도착 위치(2), 시작 시간(0), 걸리는 시간 (3).
                        var startTime = float.Parse(details[0]); //타임라인 트랙 시작 시간.

                        int startPos = CalculatePos(details[1].Trim()); //출발 위치.
                        int destination = CalculatePos(details[2].Trim()); //도착 위치.

                        var duration = int.Parse(details[3]); //도착하기 까지의 걸리는 시간.

                        GameObject characterObj = GameManager.instance.dataManager.runningCharacters[actorId].obj;
                        var animClip = GameManager.instance.timeLineBuilder.MakeAnimationClip(new Vector2(startPos, -150), new Vector2(destination, -150), duration, "Moving", 'X');
                        GameManager.instance.timeLineBuilder.BuildingTimeLine(startTime, "Move", duration, animClip, characterObj, characterTrack[actorId][0], characterTrack[actorId][1]);
                        break;
                    }


                case "Turn": //캐릭터를 좌/우로 180도 회전시킴.
                    {
                        float duration = .667f; //회전 애니메이션 재생 시간.(고정)
                        //int actorId = int.Parse(nodeSplit[0]); //타임라인에서 애니메이션을 재생할 캐릭터 ID
                        string[] details = nodeSplit[2].Split('|'); //도착 위치, 회전 방향.
                        var startTime = float.Parse(details[0]); //타임라인 트랙 시작 시간.
                        var dir = details[1].Trim(); //회전 방향. Right(왼쪽을 보던 캐릭터가 오른쪽으로 180도 회전) Left(오른쪽을 보던 캐릭터가 왼쪽으로 180도 회전)

                        var characterDatas = GameManager.instance.dataManager.runningCharacters[actorId]; //캐릭터 데이터
                        var characterPos = characterDatas.obj.GetComponent<RectTransform>().anchoredPosition; //캐릭터 위치
                        string rotationAxis = dir == "Right" ? "TurnRight" : "TurnLeft";

                        var animClip = characterDatas.characterData.characterAnim.animationClips[rotationAxis]; //캐릭터 애니메이션 클립
                        GameManager.instance.timeLineBuilder.BuildingTimeLine(startTime, "Turning", duration, animClip, characterDatas.obj, characterTrack[actorId][0], characterTrack[actorId][3]);
                        break;
                    }

                case "Fall": //지쳐서 쓰러지는 표현
                    {
                        string[] details = nodeSplit[2].Split('|');
                        var startTime = float.Parse(details[0]); //타임라인 트랙 시작 시간.

                        var characterDatas = GameManager.instance.dataManager.runningCharacters[actorId]; //캐릭터 데이터
                        var animClip = characterDatas.characterData.characterAnim.animationClips["Fall"]; //캐릭터 애니메이션 클립
                        GameManager.instance.timeLineBuilder.BuildingTimeLine(startTime, "Falling", 1.5f, animClip, characterDatas.obj, characterTrack[actorId][0], characterTrack[actorId][4]);

                        break;
                    }


                case "Walk": //걷는 것 처럼 위아래로 흔들리는 표현, 속도 조절 가능.(Loop Animation)
                    {
                        //int actorId = int.Parse(nodeSplit[0]); //타임라인에서 애니메이션을 재생할 캐릭터 ID
                        string[] details = nodeSplit[2].Split('|'); //시작 시간, 대기 시간.
                        var startTime = float.Parse(details[0]); //타임라인 기준으로 애니메이션 클립이 시작되는 시간.
                        var duration = int.Parse(details[1]); //진행 시간.

                        var characterDatas = GameManager.instance.dataManager.runningCharacters[actorId]; //캐릭터 데이터
                        var characterPos = characterDatas.obj.GetComponent<RectTransform>().anchoredPosition; //캐릭터 위치
                        var animClip = characterDatas.characterData.characterAnim.animationClips["Walk"]; //캐릭터 애니메이션 클립

                        //애니메이션 클립을 타임라인에 추가.
                        GameManager.instance.timeLineBuilder.BuildingTimeLine(startTime, "Walking", duration, animClip, characterDatas.obj, characterTrack[actorId][0], characterTrack[actorId][2]);
                        break;
                    }


                case "Fade":
                    {
                        Debug.Log("페이드 인 아웃 연출 시작");
                        break;
                    }


                default:
                    {
                        break;
                    }

            }
        }
        GameManager.instance.timeLineManager.TimeLinePlay();
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

    private int CalculatePos(string positionCode)
    {
        return positionCode switch
        {
            "A" => (((Screen.width - 360) * 0) / 4) - 780,
            "B" => (((Screen.width - 360) * 1) / 4) - 780,
            "C" => 0,
            "D" => (((Screen.width - 360) * 3) / 4) - 780,
            "E" => (((Screen.width - 360) * 4) / 4) - 780,
            _ => -1
        };
    }
}
