using UnityEngine;

public class BossEnemyTraceController : MonoBehaviour
{
    public float moveSpeed = 0.77f;
    public float raycastDistance = 0.2f;
    public float traceDistance = 2f;

    float defaultSpeed;

    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultSpeed = moveSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnDestroy()
    {
        BossController boss = FindObjectOfType<BossController>();

        if (boss != null)
        {
            boss.RemoveMob(gameObject);
        }
    }

    

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Respawn();
        }
    }

    public void SetSpeed(float multiplier)
    {
        moveSpeed = defaultSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        moveSpeed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = player.position - transform.position;

        if (direction.magnitude > traceDistance)
        {
            return;
        }

        Vector2 directionNormalized = direction.normalized;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionNormalized, raycastDistance);
        Debug.DrawRay(transform.position, directionNormalized * raycastDistance, Color.red);

        foreach(RaycastHit2D rHit in hits)
        {
            if (rHit.collider != null && rHit.collider.CompareTag("Obstacle"))
            {
                Vector3 alternativeDirection = Quaternion.Euler(0f, 0f, -90f) * direction;
                transform.Translate(alternativeDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }
    }
}
