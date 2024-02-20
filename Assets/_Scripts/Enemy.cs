using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Enemy : Character
{
    [Header("Movement")]
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;

    private Animator m_animator;
    private Rigidbody2D m_rb2D;
    private Sensor_Bandit m_groundSensor;
    private bool m_grounded = false;

    [Header("Femi's Variables")]
    [SerializeField] private HeroKnight player;
    public bool isAttacking;
    private bool hasTakenDamageThisAttack;

    public HealthBar banditHealthBar;
    public Slider slider;
    [SerializeField] float followThreshold;
    [SerializeField] float combatIdleThreshold;
    [SerializeField] float attackThreshold;
    [SerializeField] float hitDistance;
    [SerializeField] float attackCooldownThreshold;
    private float attackCooldownTimer;
    private float distanceToPlayer;


    // Enemy state system 
    enum EnemyStates
    {
        Idle,
        Track,
        CombatIdle,
        Jump,
        Attack,
        Hurt,
        Death,
    }

    [Header("State Variables")]
    [SerializeField]
    private EnemyStates currentState;
    EnemyStates previousState;
    
    // ================== State Handler ====================
    void StateHandler()
    {
        switch (currentState)
        {
            case EnemyStates.Idle:
                IdleState();
                break;
            case EnemyStates.Track:
                TrackState();
                break;
            case EnemyStates.CombatIdle:
                CombatIdleState();
                break;
            case EnemyStates.Jump:
                JumpState();
                break;
            case EnemyStates.Attack:
                AttackState();
                break;
            case EnemyStates.Hurt:
                HurtState();
                break;
            case EnemyStates.Death:
                DeathState();
                break;
        }
    }

    // ================== State Logic =======================
    void IdleState()
    {
        StateSwitcher();
        m_animator.SetInteger("AnimState", 0);
    }

    void TrackState()
    {
        StateSwitcher();

        if (m_grounded && distanceToPlayer <= combatIdleThreshold && player != null && health >= 0)
        {
            previousState = currentState;
            currentState = EnemyStates.CombatIdle;
        }

        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

        // Swap direction of sprite depending on walk direction
        if (direction > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (direction < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Move towards the player
        m_rb2D.velocity = new Vector2(direction * m_speed, 0f);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_rb2D.velocity.y);
        m_animator.SetInteger("AnimState", 2);
    }

    void CombatIdleState()
    {
        if (m_grounded && distanceToPlayer <= attackThreshold && player != null && health >= 0 && attackCooldownTimer <= 0)
        {
            previousState = currentState;
            currentState = EnemyStates.Attack;
        }

        StateSwitcher();

        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

        // Swap direction of sprite depending on walk direction
        if (direction > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (direction < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        m_rb2D.velocity = Vector2.zero;


        m_animator.SetInteger("AnimState", 1);
    }

    void JumpState()
    {
        StateSwitcher();
        m_animator.SetBool("Grounded", m_grounded);
    }

    void AttackState()
    {
        m_rb2D.velocity = Vector3.zero;
        isAttacking = true;
        m_animator.SetTrigger("Attack");
        hasTakenDamageThisAttack = false;
        attackCooldownTimer = attackCooldownThreshold;
        currentState = EnemyStates.CombatIdle;
    }

    void HurtState()
    {
        m_animator.SetTrigger("Hurt");
        health -= damagePoints;
        banditHealthBar.SetHealth(health);
        hasTakenDamageThisAttack = true;
        currentState = previousState;
    }

    void DeathState()
    {
        m_animator.SetTrigger("Death");
        m_rb2D.velocity = Vector3.zero;
        StartCoroutine(RemoveEnemy());
    }


    // =================== State Switcher ==================
    void StateSwitcher()
    {
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            currentState = previousState;
        }
        else if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            previousState = currentState;
            currentState = EnemyStates.Jump;
        }

        else if (m_grounded && distanceToPlayer <= followThreshold && distanceToPlayer > combatIdleThreshold && player != null && health >= 0)
        {
            previousState = currentState;
            currentState = EnemyStates.Track;
        }

        

        

        if (distanceToPlayer <= hitDistance && player.isAttacking && !hasTakenDamageThisAttack)
        {
            previousState = currentState;
            currentState = EnemyStates.Hurt;
        }

        if (health <= 0)
        {
            previousState = currentState;
            currentState = EnemyStates.Death;
        }
    }


    // =================== Gravity ========================
    void gravity()
    {

    }


    // ================== Timer ==============================
    void attackCooldownTime()
    {
        if( attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
    }

    // =================== Awake ==========================
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_rb2D = GetComponent<Rigidbody2D>();
        attackCooldownTimer = 0;
    }


    // =================== Start ==========================
    void Start()
    {
        currentState = EnemyStates.Idle;
        previousState = currentState;

        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();

        if (player == null)
        {
            Debug.LogError("Player not found. Please make sure the Player(HeroKnight prefab) exists and is assigned in the Inspector");
        }

        banditHealthBar.SetMaxHealth(health);
        banditHealthBar.SetHealth(health);
    }


    // ================== Update ==========================
    void Update()
    {
        if ( player != null)
        {
            distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        }

        StateHandler();
        attackCooldownTime();
    }


    // ================== Coroutines =======================
    IEnumerator RemoveEnemy()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

}
