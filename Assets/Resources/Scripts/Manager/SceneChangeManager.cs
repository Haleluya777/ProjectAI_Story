using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public void ChangeScene(int num)
    {
        SceneManager.LoadScene(num);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
