using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bandit : Character
{
    [SerializeField] private float m_speed = 4.0f;
    [SerializeField] private float m_jumpForce = 7.5f;
    private Animator bandit_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    private HeroKnight player;
    [SerializeField] private float followThreshold;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;

    [SerializeField] private LayerMask playerLayer;
    private Collider2D playerHit;
    private float attackCooldown = 1.0f;
    private bool isAttacking = false;
    private bool isDead = false;
    private float timeSinceAttack;

    private void Awake()
    {
        bandit_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        player = GameObject.FindObjectOfType<HeroKnight>();
    }

    void Start()
    {
        health = 100;
        damagePoints = 10;
    }

    void Update()
    {
        timeSinceAttack += Time.deltaTime;

        float inputX = Input.GetAxis("Horizontal");

        // if (!m_grounded && m_groundSensor.State())
        // {
        //     m_grounded = true;
        //     m_animator.SetBool("Grounded", m_grounded);
        // }

        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            bandit_animator.SetBool("Grounded", m_grounded);
        }

        // if ((player != null) && (distanceToPlayer <= followThreshold))
        // {
        //     float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

        //     if (direction > 0)
        //         transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        //     else if (direction < 0)
        //         transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        //     m_body2d.velocity = new Vector2(direction * m_speed, 0f);
        // }


        // Deal damage to player
        playerHit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
        if ((timeSinceAttack > attackCooldown) && (playerHit != null))
        {
            Attack(player);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    protected override void Attack(HeroKnight heroKnight)
    {
        if (!heroKnight.isPlayerDead && !isDead && !isAttacking)
        {
            isAttacking = true;
            bandit_animator.SetTrigger("Attack");

            // Deal damage to player
            Collider2D playerHit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
            if (playerHit != null)
            {
                playerHit.GetComponent<HeroKnight>().PlayerTakeDamage(damagePoints);
            }

            ResetAttack();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    private void ResetAttack()
    {
        isAttacking = false;
        timeSinceAttack = 0.0f;
    }

    public void BanditTakeDamage(int attackPoints)
    {
        base.TakeDamage(attackPoints);
        bandit_animator.SetTrigger("Hurt");
    }

    protected override void Die()
    {
        isDead = true;
        bandit_animator.SetTrigger("Death");
        StartCoroutine(RemoveEnemy());
    }

    IEnumerator RemoveEnemy()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
