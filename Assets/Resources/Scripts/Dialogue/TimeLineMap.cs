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

    public int Id => id;
    public string Description => description;
    public TimelineAsset Asset => asset;
}

[CreateAssetMenu(fileName = "TimeLineMap", menuName = "ScriptableObjects/TimeLine/TimeLineMap")]
public class TimeLineMap : ScriptableObject
{
    public TimeLineInfo[] timeLineInfos;

    public TimeLineInfo GetTimeLine(int id)
    {
        if (id < 0 || id > timeLineInfos.Length)
        {
            Debug.Log($"유효하지 않은 id값. ID : {id}");
            return null;
        }
        return timeLineInfos[id];
    }
}
