using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckForWin();
        }
    }

    private void CheckForWin()
    {
        Bandit _bandit = FindAnyObjectByType<Bandit>();
        if (_bandit != null)
        {
            // If there are still bandits, the player has not won yet
            Debug.Log("You must defeat all bandits to win");
            return;
        }
        GameManager.UpdateGameState(GameManager.GameState.Win);
    }
}
