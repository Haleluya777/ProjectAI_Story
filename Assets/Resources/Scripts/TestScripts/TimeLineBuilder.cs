using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Linq;
using System.Reflection;


public class TimeLineBuilder : MonoBehaviour
{
    public PlayableDirector director;
    public TimelineAsset timelineAsset;
    private Animator anim;
    private AnimationClip animClip; //빈 클립.
    public SignalAsset DialogueResumeSignal;

    private void Awake()
    {
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

    public void AddSignalInTrack(AnimationTrack track, float time, SignalAsset signal)
    {
        SignalEmitter signalEmitter = track.CreateMarker<SignalEmitter>(time);
        signalEmitter.asset = signal;
    }

    public AnimationTrack[] TrackSetting(AnimationTrack[] tracks)
    {
        tracks[0] = timelineAsset.CreateTrack<AnimationTrack>(null, "Parent"); //부모 애니메이션 트랙
        tracks[1] = timelineAsset.CreateTrack<AnimationTrack>(tracks[0], "Movement"); //캐릭터 이동 RectTransform의 X축만 담당.
        tracks[2] = timelineAsset.CreateTrack<AnimationTrack>(tracks[0], "Effect"); //위아래로 흔들리는 걷기 모션. RectTransform의 Y축만 담당.
        tracks[3] = timelineAsset.CreateTrack<AnimationTrack>(tracks[0], "Turning"); //캐릭터 회전. RectTransform.Rotation.Y만 담당.
        tracks[4] = timelineAsset.CreateTrack<AnimationTrack>(tracks[0], "Effect"); //캐릭터 쓰러짐. RectTransform의 X,Y축 둘 다 담당. 최하위 위치.

        return tracks;
    }

    public void SetAnimator(AnimationTrack parentTrack, GameObject character)
    {
        director.SetGenericBinding(parentTrack, character.GetComponent<Animator>());
    }

    public void BuildingTimeLine(float startTime, string trackName, float _duration, AnimationClip clip, GameObject character, AnimationTrack parentTrack, AnimationTrack track)
    {
        //빈 애니메이션 트랙 생성.
        //AnimationTrack track = timelineAsset.CreateTrack<AnimationTrack>(parentTrack, trackName);
        //director.SetGenericBinding(parentTrack, character.GetComponent<Animator>()); //트랙에 움직일 캐릭터 애니메이터 할당.
        var myFieldInfo = typeof(TimelineClip).GetField("m_PostExtrapolationMode", BindingFlags.NonPublic | BindingFlags.Instance);

        TimelineClip timelineClip = track.CreateClip(clip); //트랙에 클립 할당.
        timelineClip.start = startTime; //클립 시작 시간
        timelineClip.duration = _duration;

        myFieldInfo.SetValue(timelineClip, TimelineClip.ClipExtrapolation.Hold);
    }

    public AnimationClip MakeAnimationClip(Vector2 startPos, Vector2 endPos, float duration, string clipName, char attribute) //이동 애니메이션 제작.
    {
        var clip = Instantiate(animClip);
        clip.name = clipName;

        var moveX = AnimationCurve.Linear(0, startPos.x, duration, endPos.x); //AnimationCurve생성. (일종의 애니메이션 설계도.)
        var moveY = AnimationCurve.Linear(0, startPos.y, duration, endPos.y);

        clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.x", moveX); //생성한 애니메이션 클립에 설계도 할당.
        clip.SetCurve("", typeof(RectTransform), "m_AnchoredPosition.y", moveY);

        return clip;
    }
}
