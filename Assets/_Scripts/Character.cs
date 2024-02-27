using UnityEngine;

public class Character : MonoBehaviour
{
    protected const int maxHealth = 100;
    protected int health;
    [SerializeField] protected int damagePoints;
    [SerializeField] protected bool m_noBlood = true;

    private void Start()
    {
        health = maxHealth;
    }

    protected virtual void Attack(){

    }

    protected virtual void Attack(HeroKnight heroKnight){
        
    }
    

    protected virtual void TakeDamage(int attackPoints)
    {
        health -= attackPoints;
    }

    protected virtual void Die() { }
}
