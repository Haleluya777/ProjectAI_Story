using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resolutionTxt;
    //[SerializeField] private TextMeshProUGUI frameRateTxt;
    [SerializeField] private TextMeshProUGUI fullScreenTxt;

    private Resolution[] resolutions =
    {
        new Resolution {width = 1280, height = 720},
        new Resolution {width = 1920, height = 1080},
        new Resolution {width = 2560, height = 1440},
        new Resolution {width = 3840, height = 2160}
    };
    private FullScreenMode[] screenModes = { FullScreenMode.Windowed, FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow };
    private int[] frameRates = { 30, 60, 120, -1 };
    private int currentResolutionNum;
    private int currentFrame;

    private void Start()
    {
        currentResolutionNum = resolutions.Length - 1;
        currentFrame = 1;

        Screen.SetResolution(resolutions[0].width, resolutions[0].height, true);
        resolutionTxt.text = resolutions[0].ToString().Split('@')[0];

        Screen.fullScreen = false;
        fullScreenTxt.text = "Windowed";

        //Application.targetFrameRate = frameRates[currentFrame];
        //frameRateTxt.text = Application.targetFrameRate + "FPS";
    }

    public void SetResolution(int num)
    {
        currentResolutionNum += num;

        if (currentResolutionNum > resolutions.Length - 1) currentResolutionNum = 0;
        else if (currentResolutionNum < 0) currentResolutionNum = resolutions.Length - 1;

        Resolution resolution = resolutions[currentResolutionNum];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        resolutionTxt.text = resolution.ToString().Split('@')[0];
    }

    //public void SetFrameRate(int num)
    //{
    //    currentFrame += num;
    //
    //    if (currentFrame > frameRates.Length - 1) currentFrame = 0;
    //    else if (currentFrame < 0) currentFrame = frameRates.Length - 1;
    //
    //    Application.targetFrameRate = frameRates[currentFrame];
    //
    //    if (currentFrame == 3) frameRateTxt.text = "No Limit";
    //    else frameRateTxt.text = Application.targetFrameRate.ToString() + "FPS";
    //}

    public void SetFullScreen(bool value)
    {
        Screen.SetResolution(resolutions[currentResolutionNum].width, resolutions[currentResolutionNum].height, value);
        if (value) fullScreenTxt.text = "Full Screen";
        else fullScreenTxt.text = "Windowed";
    }
}
