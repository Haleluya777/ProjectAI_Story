using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeLineMap", menuName = "ScriptableObjects/TimeLine/TimeLineMap")]
public class TimeLineMap : ScriptableObject
{
    public List<TimeLineInfo> infos = new List<TimeLineInfo>();
}
