using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour, DataInitializable
{
    [SerializeField] private int actionPoint;
    [SerializeField] private int initialAP; //초기화 시킬 액션 포인트 값. 다음 턴으로 진행될 때마다 AP가 이 변수 값으로 변경됨.
    [SerializeField] private int currentTurn;

    private void Start()
    {

    }

    public void InitializeData()
    {
        actionPoint = initialAP;
        currentTurn = 1;
    }

    public void NextTurn()
    {
        currentTurn++;
        actionPoint = initialAP;
    }
}
