using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Linq;


public class TimeLineBuilder : MonoBehaviour
{
    public PlayableDirector director;
    public TimelineAsset timelineAsset;
    public GameObject character;
    private Animator anim;
    private AnimationClip animClip; //빈 클립.
    public AnimationClip walking; //걷는 것 처럼 위아래로 흔들리는 애니메이션 클립.
    private AnimationTrack parent;

    private void Awake()
    {
        anim = character.GetComponent<Animator>();
        animClip = new AnimationClip { legacy = false };

        if (timelineAsset != null)
        {
            //timelineAsset = Instantiate(timelineAsset);
            director.playableAsset = timelineAsset;
        }
    }

    private void Update()
    {
        //Debug.Log(director.state);
    }

    public void InitTimeLine()//타임라인 초기화
    {
        foreach (var track in timelineAsset.GetRootTracks().ToList())
        {
            timelineAsset.DeleteTrack(track);
        }
    }

    public AnimationTrack MakeParentTrack()
    {
        parent = timelineAsset.CreateTrack<AnimationTrack>(null, "Parent");
        return parent;
    }

    public void BuildingTimeLine(int startTime, string trackName, float _duration, AnimationClip clip, GameObject character, AnimationTrack parentTrack)
    {
        //빈 애니메이션 트랙 생성.
        AnimationTrack track = timelineAsset.CreateTrack<AnimationTrack>(parentTrack, trackName);
        director.SetGenericBinding(parentTrack, character.GetComponent<Animator>()); //트랙에 움직일 캐릭터 애니메이터 할당.

        TimelineClip timelineClip = track.CreateClip(clip); //트랙에 클립 할당.
        timelineClip.start = startTime; //클립 시작 시간
        timelineClip.duration = _duration;
    }

    public AnimationClip MakeAnimationClip(Vector2 startPos, Vector2 endPos, float duration, string clipName, char attribute)
    {
        var clip = Instantiate(animClip);
        clip.name = clipName;

        if (attribute == 'X')
        {
            var moveX = AnimationCurve.Linear(0, startPos.x, duration, endPos.x);
            clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", moveX);
        }
        else if (attribute == 'Y')
        {
            var moveY = AnimationCurve.Linear(0, startPos.y, duration, endPos.y);
            clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.y", moveY);
        }

        return clip;
    }
}
