using System.Collections;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private Animator hero_animator;
    private Rigidbody2D m_body2d;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;

    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;
    private float m_jumpForce = 7.5f;

    private float inputX;


    private void OnEnable()
    {
        GameManager.OnPlayerWin += OnPlayerWin;
        GameManager.OnPlayerLose += PlayLoseAnimation;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerWin -= OnPlayerWin;
        GameManager.OnPlayerLose -= PlayLoseAnimation;
    }

    private void OnPlayerWin()
    {
        StartCoroutine("HandlePlayerWin");
    }

    IEnumerator HandlePlayerWin()
    {
        TransitionToIdle();
        yield return new WaitForSeconds(2);
        enabled = false;
    }

    private void PlayLoseAnimation()
    {
        hero_animator.SetTrigger("Death");
    }

    private void TransitionToIdle()
    {
        m_delayToIdle -= Time.deltaTime;
        if (m_delayToIdle < 0)
            hero_animator.SetInteger("AnimState", 0);
    }
    private void Start()
    {
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        m_body2d = GetComponent<Rigidbody2D>();
        hero_animator = GetComponent<Animator>();
    }

    private void Update()
    {

        inputX = Input.GetAxis("Horizontal");


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

        //Set AirSpeed in animator
        hero_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        hero_animator.SetBool("WallSlide", m_isWallSliding);


        //Jump
        if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
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
            TransitionToIdle();
        }
    }
}