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

    //femi
    [SerializeField] private HeroKnight player;
    [SerializeField] int health = 100;
    [SerializeField] int damagePoints = 10;
    public bool isAttacking;
    private bool hasTakenDamageThisAttack;

    public HealthBar banditHealthBar;
    public Slider slider;
    [SerializeField] float followThreshold;
    [SerializeField] float attackThreshold;
    private float distanceToPlayer;
    //femi

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();

        if (player == null)
        {
            Debug.LogError("Player not found. Please make sure the Player(HeroKnight prefab) exists and is assigned in the Inspector");
        }

        banditHealthBar.SetMaxHealth(health);
        banditHealthBar.SetHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        // // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");
        
        if (player != null)
        {
            distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
        }
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }


        // Move
        // Calculate direction to move towards the player
        if ((player != null) && (player.health >= 0) && (distanceToPlayer <= followThreshold))
        {
            float direction = Mathf.Sign(player.transform.position.x - transform.position.x);

            // Swap direction of sprite depending on walk direction
            if (direction > 0)
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else if (direction < 0)
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            // Move towards the player
            m_body2d.velocity = new Vector2(direction * m_speed, 0f);
        }


        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

        // -- Handle Animations --
        //Death
        if (Input.GetKeyDown("e"))
        {
            if (!m_isDead)
                m_animator.SetTrigger("Death");
            else
                m_animator.SetTrigger("Recover");

            m_isDead = !m_isDead;
        }

        //Hurt
        else if (Input.GetKeyDown("q"))
            m_animator.SetTrigger("Hurt");

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);

        if (distanceToPlayer <= attackThreshold)
        {
            // m_body2d.velocity = Vector3.zero;

            AttackPlayer();


            //Take damage
            if (player.isAttacking && !hasTakenDamageThisAttack)
            {
                m_animator.SetTrigger("Hurt");
                health -= damagePoints;
                banditHealthBar.SetHealth(health);
                hasTakenDamageThisAttack = true;
            }
        }

        //destroy bandit when health is zero
        if (health <= 0)
        {
            m_animator.SetTrigger("Death");
            m_body2d.velocity = Vector3.zero;
            StartCoroutine(RemoveEnemy());
        }
    }
    IEnumerator RemoveEnemy()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

    private void AttackPlayer()
    {
        m_body2d.velocity = Vector3.zero;
        isAttacking = true;
        m_animator.SetTrigger("Attack");
        hasTakenDamageThisAttack = false;
    }
}
