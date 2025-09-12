using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DialoguePanel : MonoBehaviour
{
    [SerializeField] private Transform menuParent; //Dialogue 씬의 메뉴 버튼들을 총괄하는 오브젝트.
    private List<RectTransform> menuChildsPos = new List<RectTransform>();
    private bool menuClosed;
    //Dialogue Panel 하위의 UI 버튼 이벤트들 중, Dialogue Runner 스크립트와의 상호작용이 없는 버튼들의 이벤트를 담당.

    private void Start()
    {
        DialoguePanelMenuInit();
    }

    private void DialoguePanelMenuInit()
    {
        menuClosed = true;
        foreach (Transform child in menuParent)
        {
            menuChildsPos.Add(child.GetComponent<RectTransform>());
        }
    }

    public void MenuOpen()
    {
        menuClosed = !menuClosed;
        float dest = menuClosed == true ? 0 : -130;
        for (int i = 0; i < menuChildsPos.Count - 1; i++)
        {
            menuChildsPos[i].DOAnchorPosX(dest * (i + 1), .3f);
        }
    }

    public void DialogueLogClose(GameObject obj)
    {
        obj.SetActive(false);
    }
}
