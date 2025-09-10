using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;


public class DialogueFuncManager : MonoBehaviour
{
    public Dictionary<string, Action> noParamMethod = new Dictionary<string, Action>();
    public Dictionary<string, Action<object[]>> multiParamMethod = new Dictionary<string, Action<object[]>>();

    public void InitMethods()
    {
        //파라미터가 없는 메서드들 초기화
        noParamMethod.Add("Greeting", Greeting);

        //파라미터가 존재하는 메서드를 초기화
        multiParamMethod.Add("Greeting2", (param) => Greeting2(param[0].ToString(), int.Parse((string)param[1])));
        multiParamMethod.Add("Add", (param) => Add(int.Parse((string)param[0])));
        multiParamMethod.Add("MultiParamTest", (param) => MultiParamTest(param[0].ToString(), int.Parse((string)param[1])));
        multiParamMethod.Add("TimeLineRun", (param) => TimeLineRun(int.Parse((string)param[0])));
    }

    public void TimeLineRun(int timeLineId)
    {
        // 타임라인 시작 로직
        Debug.Log($"타임라인 시작 : {timeLineId}");
        Debug.Log(GameManager.instance.timeLineManager.timeLineMap.GetTimeLine(timeLineId).Asset == null);
        TimelineAsset timelineAsset = GameManager.instance.timeLineManager.timeLineMap.GetTimeLine(timeLineId).Asset; // timeLineMap에서 타임라인 가져오기
        if (timelineAsset != null)
        {
            GameManager.instance.timeLineManager.timeLine.Play(timelineAsset);
        }
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
}
