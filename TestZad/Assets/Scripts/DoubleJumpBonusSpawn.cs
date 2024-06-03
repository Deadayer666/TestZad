using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpBonusSpawn : MonoBehaviour
{
    public GameObject bonusPrefab; // Префаб объекта бонуса
    public Vector3 spawnPosition; // Координаты спавна нового бонуса

    private bool hasSpawned = false; // Флаг для отслеживания спавна бонуса

    void Start()
    {
        RespawnBonus(); // Заспавнить бонус при запуске уровня
    }

    public void RespawnBonus()
    {
        if (!hasSpawned)
        {
            GameObject newBonus = Instantiate(bonusPrefab, spawnPosition, Quaternion.identity);
            newBonus.SetActive(true);
            hasSpawned = true;
        }
    }

    // Сбросить флаг и заспавнить новый бонус с задержкой
    public void ResetSpawnedFlag()
    {
        hasSpawned = false;
        StartCoroutine(RespawnBonusAfterDelay(5f)); // Задержка 5 секунд
    }

    IEnumerator RespawnBonusAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnBonus();
    }
}