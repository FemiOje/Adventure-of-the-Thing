using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroKnight : Character
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] GameObject m_slideDust;

    private Animator hero_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    [Header("Femi's Variables")]
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] float attackCooldown;
    private float leftBound = -10.0f;
    public HealthBar playerHealthBar;
    public Slider slider;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    [SerializeField] float attackRadius;
    public bool isAttacking = false;
    private bool hasTakenDamageThisAttack;
    private float inputX;

    private void Awake()
    {
        hero_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        health = 100;
        damagePoints = 10;
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    void Update()
    {
        //set bounds
        if (transform.position.x <= leftBound)
        {
            transform.position = new Vector3(leftBound, transform.position.y, transform.position.z);
        }

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            hero_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            hero_animator.SetBool("Grounded", m_grounded);
        }



        // -- Handle input and movement -- //
        inputX = Input.GetAxis("Horizontal");

        if (!m_rolling)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }



        // -- Handle Animations --

        //Set AirSpeed in animator
        hero_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        hero_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            hero_animator.SetBool("noBlood", m_noBlood);
            hero_animator.SetTrigger("Death");
        }

        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            hero_animator.SetTrigger("Hurt");

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            hero_animator.SetTrigger("Block");
            hero_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            hero_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            hero_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }


        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            hero_animator.SetTrigger("Jump");
            m_grounded = false;
            hero_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            hero_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                hero_animator.SetInteger("AnimState", 0);
        }


        // Handle attack
        if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > attackCooldown && !m_rolling)
        {
            AttackEnemy();
        }

        //Death check
        if (health <= 0)
        {
            Die();
        }
    }

    // replace with actual attack implementation
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Bandit>())
        {
            TakeDamage(damagePoints);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish"))
        {
            StartCoroutine(WinSequence());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    void AttackEnemy()
    {
        isAttacking = true;
        
        //stop movement while attacking enemy
        inputX = 0.0f;
        m_body2d.velocity = Vector2.zero;

        m_currentAttack++;
        if (m_currentAttack > 3)
            m_currentAttack = 1;
        hero_animator.SetTrigger("Attack" + m_currentAttack);


        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
        foreach (var enemy in enemiesHit)
        {
            enemy.GetComponent<Bandit>().BanditTakeDamage(damagePoints);
        }
        StartCoroutine(ResetAttack());
    }

    protected override void TakeDamage(int attackPoints){
        base.TakeDamage(attackPoints);
        hero_animator.SetTrigger("Hurt");
    }

    protected override void Die()
    {
        GetComponent<HeroKnight>().enabled = false;
        hero_animator.SetTrigger("Death");

        //trigger death sequence
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        m_timeSinceAttack = 0.0f;
    }
    IEnumerator WinSequence()
    {
        cameraFollow.enabled = false;
        yield return new WaitForSeconds(2);
        Time.timeScale = 0.0f;
    }
}
