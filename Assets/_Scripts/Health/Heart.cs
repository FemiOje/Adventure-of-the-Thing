using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] HeroKnight player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        HeroKnight _hero = other.gameObject.GetComponent<HeroKnight>();
        if (_hero != null)
        {
            _hero.RefillHealth();
            Destroy(gameObject);
        }
    }
}
