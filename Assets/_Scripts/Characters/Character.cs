using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    protected const int maxHealth = 100;
    protected int currentHealth;
    [SerializeField] protected int damagePoints;
    [SerializeField] protected bool m_noBlood = true;
    [SerializeField] protected Slider slider;
    [SerializeField] protected Gradient gradient;
    [SerializeField] protected Image fill;


    private void Start()
    {
        InitializeHealth();
    }
    void InitializeHealth() {
        currentHealth = maxHealth;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
