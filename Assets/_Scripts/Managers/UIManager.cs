using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;

    private void OnEnable()
    {
        GameManager.OnGamePaused += ShowPauseMenu;
        GameManager.OnGameResumed += RemovePauseMenu;
        GameManager.OnPlayerWin += ShowWinMenu;
        GameManager.OnPlayerLose += ShowLoseMenu;
    }

    private void OnDisable()
    {
        GameManager.OnGamePaused -= ShowPauseMenu;
        GameManager.OnGameResumed -= RemovePauseMenu;
        GameManager.OnPlayerWin -= ShowWinMenu;
        GameManager.OnPlayerLose -= ShowLoseMenu;
    }

    public void PauseGame()
    {
        GameManager.UpdateGameState(GameManager.GameState.Paused);
    }

    public void ResumeGame()
    {
        GameManager.UpdateGameState(GameManager.GameState.Resumed);
    }

    public void ShowPauseMenu()
    {
        inGameUI?.SetActive(false);
        pauseUI?.SetActive(true);
    }

    private void RemovePauseMenu()
    {
        pauseUI?.SetActive(false);
        inGameUI?.SetActive(true);
    }

    void ShowWinMenu()
    {
        // Check if the current scene is the last scene in the build index
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            inGameUI?.SetActive(false);
            winUI?.SetActive(true);
        }
    }


    private void ShowLoseMenu()
    {
        inGameUI?.SetActive(false);
        loseUI?.SetActive(true);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
