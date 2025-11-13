using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveLoadUI : MonoBehaviour
{
    [SerializeField] private GameObject CheckingOverWritePanel;
    private GameObject contents;
    private const int SAVE_LOAD_SLOTCOUNT = 5;

    private void Awake()
    {
        contents = this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
    }

    public void SaveButtonInit()
    {
        var data = GameManager.instance.dataManager;

        for (int i = 0; i < SAVE_LOAD_SLOTCOUNT; i++)
        {
            var button = contents.transform.GetChild(i).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => Save(button, button.name.Split('_')[1]));
            //button.onClick.AddListener(() => ButtonReaction(button));
        }
    }

    public void Save(Button button, string slotNum)
    {
        if (ES3.FileExists("Save" + slotNum + ".es3"))
        {
            CheckingOverWritePanel.SetActive(true);
            var overWirteButton = CheckingOverWritePanel.transform.GetChild(1).GetChild(0).GetComponent<Button>();

            overWirteButton.onClick.AddListener(() => ES3.DeleteFile("Save" + slotNum + ".es3"));
            overWirteButton.onClick.AddListener(() => GameManager.instance.saveLoadManager.SaveGame(slotNum));
            overWirteButton.onClick.AddListener(() => ButtonReaction(button));
        }
        else
        {
            GameManager.instance.saveLoadManager.SaveGame(slotNum);
            ButtonReaction(button);
        }
    }

    public void ButtonReaction(Button button)
    {
        var proccessTxt = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var currentTimeTxt = button.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        proccessTxt.text = GameManager.instance.dataManager.proccessDatas.Day.ToString() + "번째 날 |  " + GameManager.instance.dataManager.proccessDatas.CurrentTime.ToString();
        currentTimeTxt.text = DateTime.Now.ToString(("MM/dd HH:mm"));
        //Debug.Log("시간 저장");
    }

    public void LoadButtonInit()
    {
        var data = GameManager.instance.dataManager;

        for (int i = 0; i < SAVE_LOAD_SLOTCOUNT; i++)
        {
            var button = contents.transform.GetChild(i).GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => GameManager.instance.saveLoadManager.LoadGame(button.name.Split('_')[1]));
        }
    }
}
