using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpBonusSpawn : MonoBehaviour
{
    public GameObject bonusPrefab; // ������ ������� ������
    public Vector3 spawnPosition; // ���������� ������ ������ ������

    private bool hasSpawned = false; // ���� ��� ������������ ������ ������

    void Start()
    {
        RespawnBonus(); // ���������� ����� ��� ������� ������
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

    // �������� ���� � ���������� ����� ����� � ���������
    public void ResetSpawnedFlag()
    {
        hasSpawned = false;
        StartCoroutine(RespawnBonusAfterDelay(5f)); // �������� 5 ������
    }

    IEnumerator RespawnBonusAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnBonus();
    }
}