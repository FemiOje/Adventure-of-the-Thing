using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] HeroKnight player;
    [SerializeField] HealthBar playerHealthBar;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player.health = 100;
            playerHealthBar.SetHealth(100);
            Destroy(gameObject);
        }
    }
}
