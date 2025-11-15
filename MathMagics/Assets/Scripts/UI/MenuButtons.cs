using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuButtons : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject howToPlayUI;
    public VideoPlayerBrain videoPlayerBrain;

    [Header("Audio")]
    [SerializeField] private AudioClip uiClickSFX;

    private void Awake()
    {
        if (videoPlayerBrain != null)
        { 
            videoPlayerBrain.OnVideoFinished += HandleVideoFinished;
        }
        if(mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }
        
        if(howToPlayUI != null)
        {
            howToPlayUI.SetActive(false);
        }
    }

    public void PlayButton()
    {
        mainMenuUI.SetActive(false);
        videoPlayerBrain.StartVideo();
    }

    private void HandleVideoFinished()
    {
        mainMenuUI.SetActive(false);
        howToPlayUI.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PlayUIClick()
    {
        AudioManager.Instance.PlayOneShot(uiClickSFX, 1f, AudioManager.Instance.uiAMG);
    }

    public void FadeOutMusic()
    {
        AudioManager.Instance.StopBackgroundMusic();
    }
}
