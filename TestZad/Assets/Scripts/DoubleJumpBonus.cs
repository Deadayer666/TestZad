using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpBonus : MonoBehaviour
{
    public float duration = 10f; // Длительность эффекта в секундах
    public GameObject player; // Ссылка на игрока для применения эффекта
    public Animation jumpAnimation; // Анимация прыжка

    private bool isDoubleJumpActive = false; // Флаг активности двойного прыжка
    private float remainingDuration; // Оставшаяся длительность эффекта

    private void Start()
    {
        remainingDuration = duration;
    }

    private void Update()
    {
        if (isDoubleJumpActive)
        {
            remainingDuration -= Time.deltaTime;
            if (remainingDuration <= 0)
            {
                DeactivateDoubleJump();
            }
        }
    }

    public void ActivateDoubleJump()
    {
        if (!isDoubleJumpActive)
        {
            player.GetComponent<CharacterMovement>().EnableDoubleJump();
            isDoubleJumpActive = true;
            remainingDuration = duration;
        }
        else
        {
            remainingDuration = duration;
        }
    }

    private void DeactivateDoubleJump()
    {
        player.GetComponent<CharacterMovement>().DisableDoubleJump();
        isDoubleJumpActive = false;
    }
}