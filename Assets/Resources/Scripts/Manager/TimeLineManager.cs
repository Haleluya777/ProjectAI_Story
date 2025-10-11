using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimeLineManager : MonoBehaviour
{
    public TimeLineMap timeLineMap;
    public PlayableDirector timeLine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            timeLine.Play();
        }
        //Debug.Log(timeLine.state);
    }

    public void TimeLinePlay()//int timeLineId)
    {
        timeLine.Play();
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
