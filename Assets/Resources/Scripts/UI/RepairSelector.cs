using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairSelector : MonoBehaviour
{
    //[SerializeField] private GameObject RepairSelection; //시설 수리 선택 판넬.
    [SerializeField] private Sprite repairButtonUnLockSprite; //시설 수리 버튼이 해금될 때 교체할 스프라이트.
    [SerializeField] private Transform repairButtonParent;
    [SerializeField] private GameObject Lights; //해금된 시설의 상황을 알려주는 오브젝트
    private const int REPAIRCOUNT = 5;

    public void InitRepairSelection()
    {
        Dictionary<int, EquipmentDatas> equipmentDic = GameManager.instance.dataManager.repairableEquipment;
        var equipment = GameManager.instance.dataManager.equipmentMap;

        for (int i = 0; i < REPAIRCOUNT; i++)
        {
            equipmentDic.Add(i, equipment.GetEquipment(i));

            var repairButton = repairButtonParent.GetChild(i).gameObject; //뷰 포인트 Content자식을 가져옴.
            var button = repairButton.GetComponent<Button>(); //해당 자식에서 버튼 컴포넌트 가져옴.
            var equipmentData = equipmentDic[i]; //선언한 딕셔너리에서 데이터 가져옴.

            //repairButton.transform.GetChild(0).GetComponent<Image>().sprite = equipmentData.sprite;

            button.onClick.AddListener(() => Repair(equipmentData)); //OnClick 이벤트 추가.
            button.interactable = false;

            repairButton.name = equipmentData.name;
        }
        UpdateRepairSelector();
    }

    public void UpdateRepairSelector()
    {
        if (GameManager.instance.dataManager.repairUnlock == 0) return;
        for (int i = 0; i < GameManager.instance.dataManager.repairUnlock; i++)
        {
            var light = Lights.transform.GetChild(i);
            var button = repairButtonParent.transform.GetChild(i).gameObject; //해금할 버튼 가져오기.

            light.GetComponent<Image>().color = new Color32(20, 224, 0, 255);
            button.GetComponent<Button>().interactable = true; //버튼 상호작용 가능하게.
            button.GetComponent<Image>().sprite = repairButtonUnLockSprite; //이미지 스프라이트 교체.
            button.transform.GetChild(3).gameObject.SetActive(false); //미해금 시 버튼 앞을 가리는 가림판 제거.
        }
    }

    public void Repair(EquipmentDatas data)
    {
        if (GameManager.instance.dataManager.proccessDatas.Routine <= 0) return;
        var dataManager = GameManager.instance.dataManager;
        dataManager.characterMap.GetCharacter(data.reactedCharacterID).CurrentdialogueNum |= (1 << data.progress); //관련된 캐릭터의 현 회화 번호의 다음 비트 1로 변경.
        data.progress++;
        //this.gameObject.SetActive(false);
        dataManager.ConsumeActionPoint(); //AP소모.
        GameManager.instance.uiManager.lobbyUIManager.UpdateAPSlider(); //슬라이더 업데이트

        //Actions.SetActive(true);
        //dataManager.CheckingFixedDialogue(dataManager.proccessDatas.Day, dataManager.dailyRoutine.IndexOf(dataManager.dailyRoutine.Get()));
        //수리 연출 타임라인 실행. 타임라인 끝날 때 시그널로 고정 대화 체크.
    }
}
