using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    SpriteRenderer sr;
    public BossController boss;
    
    void Awake()
    {
        boss = FindObjectOfType<BossController>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            boss.currentTrigger = null;
            boss.ClearCurrentTrigger();
            // 떨어지기 직전 위치를 리스폰으로 설정
            player.SetRespawnPoint(player.GetLastSafePosition());
            boss.StartBoss();
            Destroy(gameObject);
        }
    }
    
   
}
