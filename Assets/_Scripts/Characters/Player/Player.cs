﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Player : Character
{
    protected Rigidbody2D m_body2d;
    [SerializeField] public float m_speed = 4.0f;
    [SerializeField] protected float m_jumpForce = 7.5f;
    [SerializeField] private float m_rollForce = 6.0f;
    [SerializeField] private GameObject m_slideDust;

    private Animator hero_animator;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    [SerializeField] float attackCooldown;
    public HealthBar playerHealthBar;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    public LayerMask enemyLayer;
    private bool isAttacking = false;

    private void OnPlayerWin()
    {
        StartCoroutine("HandlePlayerWin");
    }

    IEnumerator HandlePlayerWin()
    {
        yield return new WaitForSeconds(2);
        Time.timeScale = 0;
    }
    private void OnEnable()
    {
        GameManager.OnPlayerWin += OnPlayerWin;
        GameManager.OnPlayerLose += DisablePlayer;
    }


    private void Awake()
    {
        hero_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        Time.timeScale = 1;
    }

    void Start()
    {
        currentHealth = 100;
        slider.value = currentHealth;
    }

    void Update()
    {
        m_timeSinceAttack += Time.deltaTime;

        if (m_timeSinceAttack > attackCooldown)
        {
            if (Input.GetMouseButtonDown(0))
            {
                AttackEnemy();
            }
        }
    }

    public void AttackEnemy()
    {
        if (!isAttacking)
        {
            isAttacking = true;

            // Stop player movement while attacking enemy
            m_body2d.velocity = Vector2.zero;

            // play animation
            m_currentAttack++;
            if (m_currentAttack > 3)
                m_currentAttack = 1;
            hero_animator.SetTrigger("Attack" + m_currentAttack);

            // Check if character is close enough to enemy to attack
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
            if (enemiesHit.Length > 0)
            {
                Bandit closestBandit = GetClosestBandit(enemiesHit);
                if (closestBandit != null && !closestBandit.IsDead())
                {
                    closestBandit.TakeDamage();
                    closestBandit.PlayHurtAnimation();
                    closestBandit.UpdateSlider();
                }
            }

            isAttacking = false;
            m_timeSinceAttack = 0.0f;
        }
    }

    private Bandit GetClosestBandit(Collider2D[] enemiesHit)
    {
        Bandit closestBandit = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemiesHit)
        {
            Bandit bandit = enemy.GetComponent<Bandit>();
            float distanceToBandit = Vector2.Distance(attackPoint.position, bandit.transform.position);

            if (distanceToBandit < closestDistance)
            {
                closestDistance = distanceToBandit;
                closestBandit = bandit;
            }
        }

        return closestBandit;
    }

    public void UpdateSlider() //move to player health or ui script
    {
        if (slider != null)
        {
            slider.value = currentHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

    public void TakeDamage()
    {
        currentHealth -= damagePoints;
        if (currentHealth <= 0 && !GameManager.IsPlayerDead())
        {
            GameManager.UpdateGameState(GameManager.GameState.Lose);
        }
    }

    public void PlayHurtAnimation()
    {
        hero_animator.SetTrigger("Hurt");
    }

    public void RefillHealth() // move to health script
    {
        currentHealth = 100;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    private void DisablePlayer()
    {
        enabled = false;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerWin -= OnPlayerWin;
        GameManager.OnPlayerLose -= DisablePlayer;
    }
}