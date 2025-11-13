using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Diagnostics; //디버깅용 탭 이동시킬 때 사용. 빌드 때 삭제 예정.

public class GameManager : MonoBehaviour
{
    //겜 매니저.
    //싱글톤 패턴으로 아무데서나 접근할 수 있게 함.
    //다른 매니저 클래스(이벤트 매니저, ui매니저 등)또한 변수로 접근하게 함으로써 싱글톤 패턴 더 안만들어도 됨.

    public static GameManager instance;
    public DialogueFuncManager dialogueFunc;
    public NewDialogueRunner dialogueRunner;
    public TimeLineManager timeLineManager;
    public TimeLineBuilder timeLineBuilder;
    public SystemDataManager dataManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public EventManager eventManager;
    public SaveLoadManager saveLoadManager;
    public GameObject debbuger; //디버깅 용 탭. 빌드 때 삭제 예정.

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else
        {
            Destroy(this.gameObject);
        }

        //dialogueFunc.InitMethods();
        foreach (var obj in GetComponentsInChildren<DataInitializable>())
        {
            //Debug.Log("데이터 초기화 됨.");
            obj.InitializeData();
        }
        //Debug.Log(dataManager.operandDic["Level"]);
    }
}
