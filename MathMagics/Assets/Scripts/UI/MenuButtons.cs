using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MenuButtons : MonoBehaviour
{
    public GameObject mainMenuUI;
    public VideoPlayer videoPlayer;

    public void PlayButton()
    {
        mainMenuUI.SetActive(false);
        videoPlayer.Play();
    }
}
