using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationBonus : MonoBehaviour
{
    public float rotationSpeed = 50.0f;
    public float boostDuration = 10.0f;

    private bool isCollected = false;
    private Vector3 originalPosition; // ������ ��������� ������� ������
    private BonusSpawn bonusSpawn; // ������ �� ��������� BonusSpawn

    void Start()
    {
        // ����� ��������� BonusSpawn � ����� � ������� RespawnBonus()
        bonusSpawn = FindObjectOfType<BonusSpawn>();
        if (bonusSpawn != null)
        {
            bonusSpawn.RespawnBonus();
        }
        else
        {
            Debug.LogError("BonusSpawn component not found in the scene.");
        }
    }

    void Update()
    {
        if (!isCollected)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CharacterMovement player = other.GetComponent<CharacterMovement>();
            if (player != null)
            {
                player.ApplySpeedBoost(boostDuration);
                CollectEffect();
            }
        }
    }

    void CollectEffect()
    {
        if (isCollected)
        {
            return;
        }

        isCollected = true;
        float disappearDuration = 1.0f;

        // �������� ������������
        StartCoroutine(DisappearAnimation(disappearDuration));
    }

    IEnumerator DisappearAnimation(float duration)
    {
        Vector3 targetPosition = originalPosition + Vector3.up;
        Vector3 targetScale = Vector3.zero;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            transform.localScale = Vector3.Lerp(Vector3.one, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���������� ����� ����� ��������
        Destroy(gameObject);

        // �������� ���� ������
        if (bonusSpawn != null)
        {
            bonusSpawn.ResetSpawnedFlag();
        }
    }
}