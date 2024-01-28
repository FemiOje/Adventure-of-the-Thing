using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [Header("UI Elements")]
    public GameObject controlUI;
    public GameObject pauseUI;

    public void PauseGame()
    {
        if (controlUI != null && pauseUI != null)
        {
           Debug.Log("Pause game");
            controlUI.SetActive(false);
            pauseUI.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    public void ResumeGame()
    {
        if (controlUI != null && pauseUI != null)
        {
            controlUI.SetActive(true);
            pauseUI.SetActive(false);
            Time.timeScale = 1.0f;
        }
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
