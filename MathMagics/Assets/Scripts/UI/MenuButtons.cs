using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MenuButtons : MonoBehaviour
{
    public GameObject mainMenuUI;
    public VideoPlayer videoPlayer;
    private void Awake()
    {
        videoPlayer.loopPointReached += HandleVideoFinished;
    }

    private void HandleVideoFinished(VideoPlayer source)
    {
        source.Stop();
        source.gameObject.SetActive(false);
        print("Video Done.");
    }

    public void PlayButton()
    {
        mainMenuUI.SetActive(false);
        videoPlayer.Play();
    }

    private void HandleVideoFinished()
    {

    }
}
