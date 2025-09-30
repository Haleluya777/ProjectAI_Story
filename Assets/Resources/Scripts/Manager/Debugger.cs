using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> characterProcessDebug = new List<TextMeshProUGUI>();
    [SerializeField] private TextMeshProUGUI dailyRoutineTxt;

    private void Awake()
    {

    }

    void Update()
    {
        dailyRoutineTxt.text = "현재 날 : " + GameManager.instance.dataManager.proccessDatas.Day + "\n" + "현재 시각 : " + GameManager.instance.dataManager.proccessDatas.CurrentTime + "\n" + "플레이어 위치 : " + GameManager.instance.dataManager.proccessDatas.PlayerPosition + "\n" + "남은 행동 횟수 :" + GameManager.instance.dataManager.proccessDatas.Routine;
        for (int i = 0; i < 5; i++)
        {
            characterProcessDebug[i].text = $"진행 상황 : {GameManager.instance.dataManager.characterMap.GetCharacter(i).CurrentdialogueNum}번 대화. ";
        }
    }
}
