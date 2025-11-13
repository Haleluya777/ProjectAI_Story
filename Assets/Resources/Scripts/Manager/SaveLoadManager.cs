using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//[ES3Serializable]
public class SaveLoadManager : MonoBehaviour
{
    public GameObject test;

    public void SaveGame(string slotNum)
    {
        ES3.Save<GameObject>("TestObj", GameManager.instance.dataManager.Test, "Save" + slotNum + ".es3"); //현재 상태 캔버스 저장.
        ES3.Save<SystemDataManager>("Data", GameManager.instance.dataManager, "Save" + slotNum + ".es3"); //데이터 매니저에 있는 데이터 일부 저장.
        ES3.Save<CharacterMap>("CharacterMap", GameManager.instance.dataManager.characterMap, "Save" + slotNum + ".es3"); //데이터 매니저 내부의 캐릭터 상황 저장(어떤 대화 파일을 진행 중이었는지, 몇 번째 라인까지 진행을 했는지)
        ES3.Save<TextAsset>("DialogueFile", GameManager.instance.dialogueRunner.DialogueFile, "Save" + slotNum + ".es3"); //대화 중이라면 해당 대화 스크립트 저장(대화 중이 아닌 상태에서 저장 시 null)
        ES3.Save<RepairableEquipment>("RepairProccess", GameManager.instance.dataManager.equipmentMap, "Save" + slotNum + ".es3"); //데이터 매니저 내부의 수리 상황 저장.
        ES3.Save<bool>("RunningDialogue", GameManager.instance.dialogueRunner.isRunning, "Save" + slotNum + ".es3"); //대화가 진행중이었는지 체크하는 변수 저장.
    }


    public void LoadGame(string slotNum)
    {
        if (!ES3.FileExists("Save" + slotNum + ".es3")) return;

        ES3.Load<GameObject>("TestObj", "Save" + slotNum + ".es3", test); //캔버스 오브젝트 로드
        ES3.Load<SystemDataManager>("Data", "Save" + slotNum + ".es3", GameManager.instance.dataManager); //데이터 매니저 로드
        ES3.Load<CharacterMap>("CharacterMap", "Save" + slotNum + ".es3", GameManager.instance.dataManager.characterMap); //캐릭터 맵 로드
        ES3.Load<RepairableEquipment>("RepairProccess", "Save" + slotNum + ".es3", GameManager.instance.dataManager.equipmentMap); //수리 시설 데이터맵 로드

        //ES3.Load<bool>("RunningDialogue", "Save" + slotNum + ".es3", GameManager.instance.dialogueRunner.isRunning); //대화가 진행중이었는지 체크하는 변수 로드.

        if (ES3.Load<bool>("RunningDialogue", "Save" + slotNum + ".es3") == true) //로드 후 Dialogue가 진행중이었다면
        {
            var runner = GameManager.instance.dialogueRunner;

            runner.DialogueFile = ES3.Load<TextAsset>("DialogueFile", "Save" + slotNum + ".es3");
            runner.RunDialogue(GameManager.instance.dataManager.MainCharacterData.dialogueLineNum);
        }

        //var newObj = Instantiate(ES3.Load<GameObject>("TestObj"), Vector3.zero, Quaternion.Euler(0, 0, 0), null);
        //var sceneName = ES3.Load<string>("Save2");
        //SceneManager.LoadScene(sceneName);
    }
}
