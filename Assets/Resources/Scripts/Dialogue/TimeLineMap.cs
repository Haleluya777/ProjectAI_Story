using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[System.Serializable]
public class TimeLineInfo
{
    [SerializeField] private int id;
    [SerializeField] private string description;
    [SerializeField] private TimelineAsset asset;
}

[CreateAssetMenu(fileName = "TimeLineMap", menuName = "ScriptableObjects/TimeLine/TimeLineMap")]
public class TimeLineMap : ScriptableObject
{
    public TimeLineInfo[] timeLineInfos;
}
