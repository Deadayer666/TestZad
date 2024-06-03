using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationBonus : MonoBehaviour
{
    public float rotationSpeed = 50.0f;
    public float boostDuration = 10.0f;

    private bool isCollected = false;
    private Vector3 originalPosition; // Храним начальную позицию бонуса
    private BonusSpawn bonusSpawn; // Ссылка на компонент BonusSpawn

    void Start()
    {
        // Найти компонент BonusSpawn в сцене и вызвать RespawnBonus()
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

        // Анимация исчезновения
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

        // Уничтожить бонус после анимации
        Destroy(gameObject);

        // Сбросить флаг спавна
        if (bonusSpawn != null)
        {
            bonusSpawn.ResetSpawnedFlag();
        }
    }
}