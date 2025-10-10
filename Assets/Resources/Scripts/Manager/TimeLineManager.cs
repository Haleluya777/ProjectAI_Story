using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

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
    }

    public void TimeLineAnimatorTrackInsert(Animator[] targetAnim)
    {
        var timelineAsset = timeLine.playableAsset as TimelineAsset; //타임라인에서 모든 트랙 반환.

        //제일 처음으로 등장한 애니메이션 트랙 중, 값이 바인딩 되지 않는 트랙을 반환.
        var animatorTrack = timelineAsset.GetOutputTracks().Where(t => t is AnimationTrack).Cast<AnimationTrack>().ToList();
        var unbountTrack = animatorTrack.Where(t => timeLine.GetGenericBinding(t) == null).ToList();
        if (unbountTrack.Count == 0) return;

        for (int i = 0; i < unbountTrack.Count; i++)
        {
            Debug.Log("값 들어감");
            timeLine.SetGenericBinding(unbountTrack[i], targetAnim[i]);
        }
    }

    public void TimeLinePlay(int timeLineId)
    {
        //TimelineAsset timelineAsset = timeLineMap.GetTimeLine(timeLineId).Asset; // timeLineMap에서 타임라인 가져오기
        //if (timelineAsset != null)
        //{
        //    timeLine.Play(timelineAsset);
        //}
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
