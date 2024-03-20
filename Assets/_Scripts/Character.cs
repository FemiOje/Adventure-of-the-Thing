using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    protected const int maxHealth = 100;
    protected int health;
    [SerializeField] protected int damagePoints;
    [SerializeField] protected bool m_noBlood = true;
    [SerializeField] protected Slider slider;
    [SerializeField] protected Gradient gradient;
    [SerializeField] protected Image fill;

    private void Start()
    {
        health = maxHealth;
        slider.value = health;
    }

    protected virtual void Attack(){

    }

    protected virtual void Attack(HeroKnight heroKnight){
        
    }
    

    protected virtual void TakeDamage(int attackPoints)
    {
        health -= attackPoints;
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    protected virtual void Die() { }
}
