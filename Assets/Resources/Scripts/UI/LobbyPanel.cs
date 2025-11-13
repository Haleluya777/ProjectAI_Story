using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] private RepairSelector repairSelector;
    [SerializeField] private FloorSelector floorSelector;
    [SerializeField] private Image lobbyBG;
    [SerializeField] private Slider actionPointSlider;
    [SerializeField] private TextMeshProUGUI playerPosition;
    [SerializeField] private TextMeshProUGUI day;

    private const int SAVE_LOAD_SLOTCOUNT = 5;

    void Start()
    {
        floorSelector.InitFloorSelection();
        repairSelector.InitRepairSelection();
    }

    public void PlayerPosUpdate(string pos)
    {
        playerPosition.text = pos;
    }

    public void ProccessNextDay()
    {
        var dataManager = GameManager.instance.dataManager;
        GameManager.instance.dataManager.AddTurn();
        day.text = "Day" + GameManager.instance.dataManager.proccessDatas.Day.ToString();
        actionPointSlider.value = (float)GameManager.instance.dataManager.proccessDatas.Routine / (float)GameManager.instance.dataManager.maxAP;
        //dataManager.CheckingFixedDialogue(dataManager.proccessDatas.Day, dataManager.dailyRoutine.IndexOf(dataManager.dailyRoutine.Get()));
        //수면 연출 타임라인 실행. 타임라인 끝날 때 시그널로 고정 대화 체크.
    }

    public void UpdateAPSlider()
    {
        actionPointSlider.value = (float)GameManager.instance.dataManager.proccessDatas.Routine / (float)GameManager.instance.dataManager.maxAP;
    }
}
