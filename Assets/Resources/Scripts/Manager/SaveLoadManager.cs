using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public void SaveGame()
    {
        Debug.Log("데이터 저장.");
        ES3.Save("SystemDatas", GameManager.instance.dataManager);
    }


    public void LoadGame()
    {
        ES3.Load("SystemDatas");
    }
}
