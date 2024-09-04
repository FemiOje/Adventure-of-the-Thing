using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Win,
        Resumed,
        Paused,
        Lose
    }

    private static GameState _currentGameState;
    private static bool isPlayerDead;
    private static bool hasGameResumed;
    public static event Action OnPlayerWin;
    public static event Action OnPlayerLose;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;


    private void OnEnable()
    {
        OnPlayerLose += HandlePlayerLose;
        OnPlayerWin += HandlePlayerWin;
        OnGamePaused += HandleGamePaused;
        OnGameResumed += ResumeGame;
    }


    IEnumerator LoadNextLevel()
    {
        bool _isNotLastLevel = SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1;
        if (_isNotLastLevel)
        {
            yield return new WaitForSeconds(2);

            // Log current and next scene index
            int _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int _nextSceneIndex = _currentSceneIndex + 1;
            Debug.Log("Current Scene Index: " + _currentSceneIndex);
            Debug.Log("Next Scene Index: " + _nextSceneIndex);

            SceneManager.LoadScene(_nextSceneIndex);
        }
        else
        {
            Debug.Log("Already at the last level.");
        }
    }

    void HandlePlayerWin()
    {
        Debug.Log("Player Won. Handling win...");
        StartCoroutine(LoadNextLevel());
    }

    private void Start()
    {
        // Log the start of the game and current scene
        Debug.Log("Game Started. Current Scene Index: " + SceneManager.GetActiveScene().buildIndex);
        UpdateGameState(GameState.Resumed);
    }


    // void HandlePlayerWin()
    // {
    //     StartCoroutine("LoadNextLevel");
    // }

    // IEnumerator LoadNextLevel()
    // {
    //     bool _isNotLastLevel = SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1;
    //     if (_isNotLastLevel)
    //     {
    //         yield return new WaitForSeconds(2);
    //         int _nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
    //         SceneManager.LoadScene(_nextSceneIndex);
    //     }
    // }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        isPlayerDead = false;
    }

    // private void Start()
    // {
    //     UpdateGameState(GameState.Resumed);
    // }


    public static void UpdateGameState(GameState _newState)
    {
        SetCurrentGameState(_newState);
        switch (_currentGameState)
        {
            case GameState.Win:
                OnPlayerWin?.Invoke();
                break;
            case GameState.Resumed:
                OnGameResumed?.Invoke();
                break;
            case GameState.Paused:
                OnGamePaused?.Invoke();
                break;
            case GameState.Lose:
                OnPlayerLose?.Invoke();
                break;
        }
    }

    public void NewGame()
    {
        int _nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(_nextSceneIndex);
    }

    public void ResumeGame()
    {
        if (!hasGameResumed)
        {
            Time.timeScale = 1;
            hasGameResumed = true;
        }
    }

    private static void SetCurrentGameState(GameState newState)
    {
        _currentGameState = newState;
    }

    private void HandleGamePaused()
    {
        Time.timeScale = 0;
        hasGameResumed = false;
    }


    private void HandlePlayerLose()
    {
        isPlayerDead = true;
    }

    public static bool IsPlayerDead()
    {
        return isPlayerDead;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Debug.Log("Loading main menu");
        SceneManager.LoadScene(0);
    }

    private void OnDisable()
    {
        OnPlayerLose -= HandlePlayerLose;
        OnGamePaused -= HandleGamePaused;
        OnGameResumed -= ResumeGame;
    }
}
