using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D m_body2d;
    private float m_speed = 4.0f;
    private SpriteRenderer m_spriteRenderer;
    private bool _canMove;
    private float inputX;
    private float leftBound = -10.0f;
    private float rightBound = 120.0f;


    private void OnEnable()
    {
        GameManager.OnPlayerWin += HandleWin;
        GameManager.OnPlayerLose += HandleLose;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerWin -= HandleWin;
        GameManager.OnPlayerLose -= HandleLose;
    }

    private void HandleWin()
    {
        StartCoroutine("WaitAndDisableMovement");
    }

    private void HandleLose()
    {
        DisableMovement();
    }

    IEnumerator WaitAndDisableMovement()
    {
        yield return new WaitForSeconds(2);
        DisableMovement();
    }

    private void DisableMovement()
    {
        _canMove = false;
    }

    private void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        _canMove = true;
    }

    private void Update()
    {
        HandleMovement();
        CheckBounds();
    }

    private void HandleMovement()
    {
        if (!_canMove)
        {
            return;
        }

        inputX = Input.GetAxis("Horizontal");
        m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        m_spriteRenderer.flipX = inputX < 0 ? true : false;
    }

    private void CheckBounds()
    {
        // Set bounds
        if (transform.position.x <= leftBound)
        {
            transform.position = new Vector3(leftBound, transform.position.y, transform.position.z);
        }

        if (transform.position.x >= rightBound)
        {
            transform.position = new Vector3(rightBound, transform.position.y, transform.position.z);
        }
    }
}
