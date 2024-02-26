using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : HeroKnight
{
    [SerializeField] HeroKnight player;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
