using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[System.Serializable]
public class TimeLineInfo : MonoBehaviour
{
    [Header("TimeLineBaseInfo")]
    public string TimeLineName;
    public string TimeLineExplanation;

    [Header("TimeLineAsset")]
    public TimelineAsset timeLine;
}
