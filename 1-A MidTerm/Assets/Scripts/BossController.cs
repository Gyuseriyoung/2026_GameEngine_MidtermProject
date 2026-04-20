using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject bossObject;
    public GameObject mobPrefab;
    public Transform[] spawnPoints;
    public GameObject lightPrefab;
    public Transform lightSpawnPoint;

    public float spawnInterval = 1.7f;     
    public float surviveTime =+ 2f;         // 더 오래 버텨야 함

    

    bool isLightActive = false;
    bool isBossRunning = false;

    List<GameObject> spawnedMobs = new List<GameObject>();
    Coroutine bossRoutine;

    public void StartBoss()
    {
        if (isBossRunning) return;

        isBossRunning = true;
        bossRoutine = StartCoroutine(BossLoop());
    }

    public void RemoveMob(GameObject mob)
    {
        if (spawnedMobs.Contains(mob))
        {
            spawnedMobs.Remove(mob);
        }
    }

    public void OnLightCollected()
    {
        isLightActive = false;
    }

    IEnumerator BossLoop()
    {
        while (true) // 반복 구조
        {
            // 1. 버티기 페이즈
            float timer = 0f;

            while (timer < surviveTime)
            {
                SpawnMob();

                yield return new WaitForSeconds(spawnInterval);
                timer += spawnInterval;
            }

            // 2. 잡몹 정리
            ClearMobs();

            // 3. 빛 생성
            SpawnLight();
            isLightActive = true;

            // 4. 플레이어가 먹을 때까지 대기
            yield return new WaitUntil(() => isLightActive == false);

            // 5. 다음 웨이브 전에 잠깐 텀
            yield return new WaitForSeconds(2f);
        }
    }

    void SpawnMob()
    {
        int rand = Random.Range(0, spawnPoints.Length);

        GameObject mob = Instantiate(mobPrefab, spawnPoints[rand].position, Quaternion.identity);
        spawnedMobs.Add(mob);
    }

    void ClearMobs()
    {
        for (int i = spawnedMobs.Count - 1; i >= 0; i--)
        {
            if (spawnedMobs[i] != null)
            {
                Destroy(spawnedMobs[i]);
            }
        }

        spawnedMobs.Clear();
    }


    void SpawnLight()
    {
        if (isLightActive) return;

        Instantiate(lightPrefab, lightSpawnPoint.position, Quaternion.identity);
        isLightActive = true;
    }
}