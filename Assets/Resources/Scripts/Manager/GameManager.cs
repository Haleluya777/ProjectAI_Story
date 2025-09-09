using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //겜 매니저.
    //싱글톤 패턴으로 아무데서나 접근할 수 있게 함.
    //다른 매니저 클래스(이벤트 매니저, ui매니저 등)또한 변수로 접근하게 함으로써 싱글톤 패턴 더 안만들어도 됨.
    public static GameManager instance;

    public DialogueFuncManager dialogueFunc;

    public Dictionary<string, object> operandDic = new Dictionary<string, object>();
    public List<DialogueParser.ParsedLine> dialogueLog = new List<DialogueParser.ParsedLine>();

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

        OperandInit(operandDic);
        dialogueFunc.InitMethods();
    }

    private void OperandInit(Dictionary<string, object> dic)
    {
        dic.Add("Level", 15);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale = 5f;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            Time.timeScale = 1f;
        }
    }
}
