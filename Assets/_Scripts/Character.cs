using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected const int maxHealth = 100;
    protected int health;
    [SerializeField] protected int damagePoints;


    protected void TakeDamage(int damagePoints){

    }

    protected void Attack(){

    }


    protected void Die()
    {
        // m_animator.SetBool("noBlood", m_noBlood);
        // m_animator.SetTrigger("Death");

        // yield return new WaitForSeconds(0.5f);
        // loseUI.SetActive(true);
        // Time.timeScale = 0.0f;
    }
}
