using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject bossObject;
    public GameObject mobPrefab;
    public GameObject lightPrefab;
    public GameObject lightTriggerPrefab;
    public Transform[] spawnPoints;
    public Transform lightSpawnPoint;
    public Transform lightTriggerSpawnPoint;

    public float spawnInterval = 1.7f;     
    public float surviveTime =+ 2f;         // 더 오래 버텨야 함


    bool isLightActive = false;
    bool isBossRunning = false;
    bool isWatchActive = false;
    float currentSlowMultiplier = 1f;

    List<GameObject> spawnedItems = new List<GameObject>();
    List<GameObject> spawnedMobs = new List<GameObject>();
    Coroutine bossRoutine;
    Coroutine watchRoutine;


    [Header("Item Spawn")]
    public GameObject[] itemPrefabs;      // 여러 아이템 프리팹
    public Transform[] itemSpawnPoints;   // 스폰 위치들

    public void StartBoss()
    {
        if(bossRoutine != null)
        StopCoroutine(bossRoutine);

        bossRoutine = StartCoroutine(BossLoop());
    }

    public void RemoveMob(GameObject mob)
    {
        if (spawnedMobs.Contains(mob))
        {
            spawnedMobs.Remove(mob);
        }
    }

    public void ResetBoss()
    {
        // 코루틴 중지
        if (bossRoutine != null)
        {
            StopCoroutine(bossRoutine);
            bossRoutine = null;
        }
            

        // 몹 정리
        ClearMobs();

        // 아이템 정리
        ClearItems();

        // 상태 초기화
        isLightActive = false;
        isBossRunning = false;

        // LightTrigger 다시 생성
        SpawnLightTrigger();
    }

    public void ActivateWatch(float multiplier, float duration)
    {
        if (watchRoutine != null)
            StopCoroutine(watchRoutine);

        watchRoutine = StartCoroutine(WatchCoroutine(multiplier, duration));
    }

    public void OnLightCollected()
    {
        isLightActive = false;
    }

    void SpawnAllItems()
    {
        if (itemPrefabs.Length == 0 || itemSpawnPoints.Length == 0)
            return;

        List<int> usedSpawnIndex = new List<int>();

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            int spawnIndex;

            // 위치 겹치지 않게
            do
            {
                spawnIndex = Random.Range(0, itemSpawnPoints.Length);
            }
            while (usedSpawnIndex.Contains(spawnIndex));

            usedSpawnIndex.Add(spawnIndex);

            GameObject item = Instantiate(itemPrefabs[i], itemSpawnPoints[spawnIndex].position, Quaternion.identity);
            spawnedItems.Add(item);
        }
    }

    void ClearItems()
    {
        foreach (var item in spawnedItems)
        {
            if (item != null)
                Destroy(item);
        }

        spawnedItems.Clear();
    }

    IEnumerator BossLoop()
    {
        while (true) // 반복 구조
        {
            SpawnAllItems(); // 웨이브 시작시 랜덤 아이템 생성

            // 1. 버티기 페이즈
            float timer = 0f;

            while (timer < surviveTime)
            {
                SpawnMob();

                yield return new WaitForSeconds(spawnInterval);
                timer += spawnInterval;
            }

            // 2. 잡몹, 아이템 정리
            ClearMobs();
            ClearItems();

            // 3. 빛 생성
            SpawnLight();
            isLightActive = true;

            // 4. 플레이어가 먹을 때까지 대기
            yield return new WaitUntil(() => isLightActive == false);

            // 5. 다음 웨이브 전에 잠깐 텀
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator WatchCoroutine(float multiplier, float duration)
    {
        isWatchActive = true;
        currentSlowMultiplier = multiplier;

        ApplySlowToAll();

        yield return new WaitForSeconds(duration);

        isWatchActive = false;
        currentSlowMultiplier = 1f;

        ResetAllSpeed();
    }

    void ApplySlowToAll()
    {
        foreach (var mob in spawnedMobs)
        {
            if (mob != null)
                mob.GetComponent<BossEnemyTraceController>().SetSpeed(currentSlowMultiplier);
        }
    }

    void ResetAllSpeed()
    {
        foreach (var mob in spawnedMobs)
        {
            if (mob != null)
                mob.GetComponent<BossEnemyTraceController>().ResetSpeed();
        }
    }

    void SpawnMob()
    {
        int rand = Random.Range(0, spawnPoints.Length);

        GameObject mob = Instantiate(mobPrefab, spawnPoints[rand].position, Quaternion.identity);

        spawnedMobs.Add(mob);

        // 디버프 적용 중이면 새 몹도 느리게
        if (isWatchActive)
        {
            mob.GetComponent<BossEnemyTraceController>().SetSpeed(currentSlowMultiplier);
        }
    }

    void ClearMobs()
    {
        

        foreach (var mob in spawnedMobs)
        {
            if (mob != null)
                Destroy(mob);
        }

        spawnedMobs.Clear();
    }

    public void ClearCurrentTrigger()
    {
        currentTrigger = null;
    }

    public GameObject currentTrigger;

    void SpawnLightTrigger()
    {
        if (currentTrigger != null)
            return;
        currentTrigger = Instantiate(lightTriggerPrefab, lightTriggerSpawnPoint.position, Quaternion.identity);
    }

    void Start()
    {
        SpawnLightTrigger();
    }

    void SpawnLight()
    {
        if (isLightActive) return;

        Instantiate(lightPrefab, lightSpawnPoint.position, Quaternion.identity);
        isLightActive = true;
    }
}