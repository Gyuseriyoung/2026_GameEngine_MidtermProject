using UnityEngine;

public class LightItem : MonoBehaviour
{
    public BossController boss;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().AddLight();

            boss.OnLightCollected(); // 보스에게 알림

            Destroy(gameObject);
        }
    }
}
