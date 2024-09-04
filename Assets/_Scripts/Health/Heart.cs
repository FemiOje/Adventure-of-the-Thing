using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [SerializeField] Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player _hero = other.gameObject.GetComponent<Player>();
        if (_hero != null)
        {
            _hero.RefillHealth();
            Destroy(gameObject);
        }
    }
}
