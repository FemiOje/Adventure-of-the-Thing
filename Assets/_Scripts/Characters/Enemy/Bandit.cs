using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Bandit : Character
{
    [SerializeField] private float m_speed = 4.0f;
    [SerializeField] private float m_jumpForce = 7.5f;
    private Animator bandit_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    private HeroKnight _player;
    [SerializeField] private float followThreshold;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;

    [SerializeField] private LayerMask playerLayer;
    private Collider2D _hit;
    private float attackCooldown = 2f;
    private float timeSinceAttack;
    private bool isDead = false;

    private void Awake()
    {
        bandit_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        _player = FindObjectOfType<HeroKnight>();
    }

    private void Start()
    {
        currentHealth = 100;
    }

    private void Update()
    {
        timeSinceAttack += Time.deltaTime;

        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            bandit_animator.SetBool("Grounded", m_grounded);
        }

        // Check if the bandit can attack the player
        _hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
        if (timeSinceAttack >= attackCooldown && _hit != null && !isDead)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        bandit_animator.SetTrigger("Attack");
        _player.PlayHurtAnimation();
        _player.TakeDamage();
        _player.UpdateSlider();
        timeSinceAttack = 0f;
    }

    public void PlayHurtAnimation()
    {
        bandit_animator.SetTrigger("Hurt");
    }

    public void TakeDamage()
    {
        currentHealth -= damagePoints;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void UpdateSlider()
    {
        if (slider != null)
        {
            slider.value = currentHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

    public void Die()
    {
        isDead = true;
        bandit_animator.SetTrigger("Death");
        StartCoroutine(DestroyEnemy());
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

    public bool IsDead() {
        return isDead;
    }
}
