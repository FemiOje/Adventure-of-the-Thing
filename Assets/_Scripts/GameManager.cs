using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState {
        Win,
        InGame,
        Lose
    }

    public static GameState CurrentGameState;

    private static bool isPlayerDead;
    public static event Action OnPlayerWin;
    public static event Action OnPlayerLose;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        isPlayerDead = false;
        SetCurrentGameState(GameState.InGame);
    }

    private void OnEnable() {
        OnPlayerWin += PlayerWin;
        OnPlayerLose += PlayerLose;
    }


    private void Update() {
        switch (CurrentGameState){
            case GameState.Win:
                // Win screen logic
                OnPlayerWin?.Invoke();
                break;
            case GameState.InGame:
                // In-game logic
                break;
            case GameState.Lose:
                // Lose screen logic
                OnPlayerLose?.Invoke();
                break;
        }
    }

    public void LoadScene1()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public static void SetCurrentGameState(GameState newState) {
        CurrentGameState = newState;
    }

    private void PlayerWin()
    {
        Debug.Log("Player wins!");
        Time.timeScale = 0;
    }

    private void PlayerLose()
    {
        isPlayerDead = true;
        GetComponent<HeroKnight>().enabled = false;
        Debug.Log("Player loses!");
    }

    public static bool IsPlayerDead() {
        return isPlayerDead;
    }

    private void OnDisable() {
        OnPlayerWin -= PlayerWin;
        OnPlayerLose -= PlayerLose;
    }
}
