using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float InfinityTime = 7f;
    public float blinkInterval = 0.1f;
    public GameObject clearObjectPrefab;
    public Transform clearSpawnPoint;

    private float defaultMoveSpeed;
    private float defaultJumpForce;
    private Animator pAni;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    private int lightCount = 0;
    
    Coroutine InvicibleCoroutine;
    Coroutine speedRoutine;
    Coroutine jumpRoutine;
    SpriteRenderer sr;

    private bool isInfinity = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            if (!isInfinity)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
        }

        if (collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }

        if (collision.CompareTag("Enemy"))
        {
            if (!isInfinity)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
        }

        if (collision.CompareTag("Infinity"))
        {
            ActivateInfinity();
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Quick"))
        {
            ActivateSpeed(1.5f, 7f); // 1.5배, 3초
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Jumping"))
        {
            ActivateJump(1.5f, 7f);
            Destroy(collision.gameObject);
        }

        
    }
        
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput < 0)
        {
            transform.localScale = new Vector3(-0.2f,0.2f,0.2f);
        }
        else if (moveInput > 0)
        {
            transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void AddLight()
    {
        lightCount++;

        if (lightCount >= 6)
        {
            SpawnClearObject();
        }
    }

    void SpawnClearObject()
    {
        Instantiate(clearObjectPrefab, clearSpawnPoint.position, Quaternion.identity);
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            pAni.SetTrigger("Jump");
        }
    }
    
    IEnumerator InfinityCoroutine()
    {
        isInfinity = true;

        float timer = 0f;

        while (timer < InfinityTime)
        {
            sr.enabled = !sr.enabled; // 깜빡임

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        sr.enabled = true; // 끝날 때 다시 보이게
        isInfinity = false;
    }

    IEnumerator SpeedUpCoroutine(float multiplier, float duration)
    {
        moveSpeed = defaultMoveSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = defaultMoveSpeed;
    }

    IEnumerator JumpUpCoroutine(float multiplier, float duration)
    {
        jumpForce = defaultJumpForce * multiplier;

        yield return new WaitForSeconds(duration);

        jumpForce = defaultJumpForce;
    }

    void ActivateInfinity()
    {
        if (InvicibleCoroutine != null)
        {
            StopCoroutine(InvicibleCoroutine);
        }

        InvicibleCoroutine = StartCoroutine(InfinityCoroutine());
    }

    void ActivateSpeed(float multiplier, float duration)
    {
        if (speedRoutine != null)
            StopCoroutine(speedRoutine);

        speedRoutine = StartCoroutine(SpeedUpCoroutine(multiplier, duration));
    }

    void ActivateJump(float multiplier, float duration)
    {
        if (jumpRoutine != null)
            StopCoroutine(jumpRoutine);

        jumpRoutine = StartCoroutine(JumpUpCoroutine(multiplier, duration));
    }


}
