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
    public AnimationClip animClip;

    private void Awake()
    {
        anim = character.GetComponent<Animator>();
    }

    private void Update()
    {
        //director = GetComponent<PlayableDirector>();
        //timelineAsset = director.playableAsset as TimelineAsset;
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("할렐루야!");
            foreach (var track in timelineAsset.GetRootTracks().ToList())
            {
                timelineAsset.DeleteTrack(track);
            }

            AnimationTrack animTrack = timelineAsset.CreateTrack<AnimationTrack>(null, "PlayerAnim");
            director.SetGenericBinding(animTrack, anim);

            ControlTrack controlTrack = timelineAsset.CreateTrack<ControlTrack>(null, "PlayerControl");
            director.SetGenericBinding(controlTrack, character);

            TimelineClip newClip = animTrack.CreateClip(animClip);
            newClip.start = 0;
            newClip.duration = 20;

            TimelineClip moveClip = controlTrack.CreateDefaultClip();
            moveClip.duration = 2.0;
            moveClip.start = 0;
        }
    }
}
