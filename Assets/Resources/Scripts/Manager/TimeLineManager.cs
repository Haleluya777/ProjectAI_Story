using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimeLineManager : MonoBehaviour
{
    public TimeLineMap timeLineMap;
    public PlayableDirector timeLine;

    void Update()
    {
        Debug.Log(timeLine.state);
    }

    public void TimeLinePlay(int timeLineId)
    {
        TimelineAsset timelineAsset = timeLineMap.GetTimeLine(timeLineId).Asset; // timeLineMap에서 타임라인 가져오기
        if (timelineAsset != null)
        {
            timeLine.Play(timelineAsset);
        }
    }

    public void TimeLinePause()
    {
        timeLine.Pause();
    }

    public void TimeLineResume()
    {
        timeLine.Resume();
    }
}
