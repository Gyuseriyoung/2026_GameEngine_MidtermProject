using UnityEngine;

public class ItemWatch : MonoBehaviour
{
    public BossController boss;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            boss.ActivateWatch(0.5f, 5f); // 속도 50%, 5초

            Destroy(gameObject);
        }
    }

    void Awake()
    {
        boss = FindObjectOfType<BossController>();  //프리팹에서 보스 오브젝트를 못넣어서 작성한 코드
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
