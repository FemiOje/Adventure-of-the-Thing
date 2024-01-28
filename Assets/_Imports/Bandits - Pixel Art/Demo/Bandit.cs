using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bandit : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    private bool m_combatIdle = false;
    private bool m_isDead = false;

    [SerializeField] private HeroKnight player;
    [SerializeField] int health = 100;
    [SerializeField] int damagePoints = 10;
    public bool isAttacking;
    private bool hasTakenDamageThisAttack;

    public HealthBar banditHealthBar;
    [SerializeField] float followThreshold;
    [SerializeField] float attackThreshold;
    private float distanceToPlayer;
    float attackCooldown = 0.3f; // Reduced cooldown for faster attacks
    private bool canAttack = true;

    [SerializeField] float fallSpeedMultiplier = 1.0f;
    [SerializeField] float m_gravityScale = 1.0f;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        m_body2d.gravityScale = m_gravityScale;
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();

        if (player == null)
        {
            Debug.LogError("Player not found. Please make sure the Player(HeroKnight prefab) exists and is assigned in the Inspector");
        }

        banditHealthBar.SetMaxHealth(health);
        banditHealthBar.SetHealth(health);
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");

        if (player != null)
        {
            distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        }

        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if ((player != null) && (player.health >= 0) && (distanceToPlayer <= followThreshold))
        {
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

            if (direction > 0)
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else if (direction < 0)
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            m_body2d.velocity = new Vector2(direction * m_speed, 0f);
        }

        if (distanceToPlayer <= attackThreshold)
        {
            if (canAttack)
            {
                AttackPlayer();
                StartCoroutine(AttackCooldown());
            }

            if (player.isAttacking && !hasTakenDamageThisAttack)
            {
                m_animator.SetTrigger("Hurt");
                health -= damagePoints;
                banditHealthBar.SetHealth(health);
                hasTakenDamageThisAttack = true;
            }
        }

        if (health <= 0)
        {
            m_animator.SetTrigger("Death");
            m_body2d.velocity = Vector3.zero;
            StartCoroutine(RemoveEnemy());
        }
    }

    // Corrected part: Deduct health from the bandit when attacked by the player
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!hasTakenDamageThisAttack)
            {
                m_animator.SetTrigger("Hurt");
                health -= damagePoints;
                banditHealthBar.SetHealth(health);
                hasTakenDamageThisAttack = true;
            }
        }
    }

    private void AttackPlayer()
    {
        player.health -= 1;
        m_body2d.velocity = Vector3.zero;
        isAttacking = true;
        m_animator.SetTrigger("Attack");
        hasTakenDamageThisAttack = false;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator RemoveEnemy()
    {
        yield return new WaitForSeconds(1.0f);
        m_animator.SetTrigger("Death");
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

}
