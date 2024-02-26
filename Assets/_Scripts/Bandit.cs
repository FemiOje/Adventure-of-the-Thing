using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bandit : Character
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    private Animator bandit_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;
    [SerializeField] private HeroKnight player;
    public bool isAttacking;
    [SerializeField] float followThreshold;
    [SerializeField] float attackThreshold;
    private float distanceToPlayer;
    float attackCooldown = 0.3f; // Reduced cooldown for faster attacks

    private void Awake()
    {
        bandit_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        health = 100;
        damagePoints = 10;
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");

        if (player != null)
        {
            distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        }

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

        if ((player != null) && (distanceToPlayer <= followThreshold))
        {
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

            if (direction > 0)
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else if (direction < 0)
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            m_body2d.velocity = new Vector2(direction * m_speed, 0f);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int attackPoints){
        base.TakeDamage(attackPoints);
        bandit_animator.SetTrigger("Hurt");
    }

    protected override void Die()
    {
        bandit_animator.SetTrigger("Death");
        m_body2d.velocity = Vector3.zero;
        StartCoroutine(RemoveEnemy());
    }

    IEnumerator RemoveEnemy()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
