using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpBonus : MonoBehaviour
{
    public float rotationSpeed = 50.0f;
    public float duration = 10f;
    private bool isCollected = false;
    private Vector3 originalPosition; // Храним начальную позицию бонуса
    private DoubleJumpBonusSpawn bonusSpawn; // Ссылка на компонент BonusSpawn

    public GameObject player;
    private CharacterMovement characterMovement;

    private void Start()
    {
        if (player != null)
        {
            characterMovement = player.GetComponent<CharacterMovement>();
        }
        originalPosition = transform.position;

        // Найти компонент BonusSpawn в сцене и вызвать RespawnBonus()
        bonusSpawn = FindObjectOfType<DoubleJumpBonusSpawn>(); // Найти DoubleJumpBonusSpawn
        if (bonusSpawn != null)
        {
            bonusSpawn.RespawnBonus();
        }
        else
        {
            Debug.LogError("BonusSpawn component not found in the scene.");
        }
    }

    private void Update()
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
                player.EnableDoubleJump(duration);
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