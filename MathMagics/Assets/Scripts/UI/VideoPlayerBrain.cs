using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerBrain : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public event Action OnVideoFinished;
    public GameObject skipButton;

    private void Awake()
    {
        videoPlayer.loopPointReached += HandleVideoFinished;
    }

    private void HandleVideoFinished(VideoPlayer source)
    {
        videoPlayer.Stop();
        videoPlayer.gameObject.SetActive(false);
        skipButton.SetActive(false);
        OnVideoFinished?.Invoke();

    }

    public void StartVideo()
    {
        videoPlayer.Play();
        skipButton.SetActive(true);
    }

    public void SkipVideo()
    {
        videoPlayer.Stop();
        skipButton.SetActive(false);
        HandleVideoFinished(videoPlayer);
    }
}
