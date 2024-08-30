using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    public Gradient gradient;
    public Image fill;
    // [SerializeField] HeroKnight player;

    // public void SetMaxHealth(Slider slider, int newHealth)
    // {
    //     slider.maxValue = maxHealth;
    //     slider.value = maxHealth;
    //     fill.color = gradient.Evaluate(1f);
    // }

    public void SetHealth(Slider slider, int newHealth)
    {
        slider.value = newHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}
