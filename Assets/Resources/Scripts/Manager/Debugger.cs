using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Hallelujah;

public class Debugger : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> characterProcessDebug = new List<TextMeshProUGUI>();
    [SerializeField] private TextMeshProUGUI dailyRoutineTxt;
    private List<string> time = new List<string> { "아침", "오후", "저녁" };
    private CirclularList<string> circularTime;

    private void Awake()
    {
        circularTime = new CirclularList<string>(time);

    }


}
