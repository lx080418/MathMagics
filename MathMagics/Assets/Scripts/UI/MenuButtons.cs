using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject howToPlayUI;
    public VideoPlayerBrain videoPlayerBrain;

    private void Awake()
    {
        videoPlayerBrain.OnVideoFinished += HandleVideoFinished;
        mainMenuUI.SetActive(true);
        howToPlayUI.SetActive(false);
        
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
}
