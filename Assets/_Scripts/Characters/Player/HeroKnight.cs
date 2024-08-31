using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class HeroKnight : Character
{
    [SerializeField] private float m_speed = 4.0f;
    [SerializeField] private float m_jumpForce = 7.5f;
    [SerializeField] private float m_rollForce = 6.0f;
    [SerializeField] private GameObject m_slideDust;

    private Animator hero_animator;
    private Rigidbody2D m_body2d;
    private SpriteRenderer m_spriteRenderer;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;

    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] float attackCooldown;
    private float leftBound = -10.0f;
    public HealthBar playerHealthBar;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    public LayerMask enemyLayer;
    private bool isAttacking = false;
    private float inputX;

    private void Awake()
    {
        hero_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = 100;
        slider.value = currentHealth;
    }

    void Update()
    {
        m_timeSinceAttack += Time.deltaTime;

        // Set bounds
        if (transform.position.x <= leftBound)
        {
            transform.position = new Vector3(leftBound, transform.position.y, transform.position.z);
        }

        Move();

        if (m_timeSinceAttack > attackCooldown)
        {
            if (Input.GetMouseButtonDown(0))
            {
                AttackEnemy();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            CheckWin();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void Move()
    {
        inputX = Input.GetAxis("Horizontal");

        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        if (inputX > 0)
        {
            m_spriteRenderer.flipX = false;
        }

        if (inputX < 0)
        {
            m_spriteRenderer.flipX = true;
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

    public void UpdateSlider()
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
            GameManager.SetCurrentGameState(GameManager.GameState.Lose);
        }
    }

    public void PlayHurtAnimation()
    {
        hero_animator.SetTrigger("Hurt");
    }

    public void RefillHealth()
    {
        currentHealth = 100;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    private void CheckWin()
    {
        Bandit _bandit = FindAnyObjectByType<Bandit>();
        if (_bandit != null)
        {
            // If there are still bandits, the player has not won yet
            Debug.Log("You must defeat all bandits to win");
            return;
        }
        GameManager.SetCurrentGameState(GameManager.GameState.Win);
    }



}
