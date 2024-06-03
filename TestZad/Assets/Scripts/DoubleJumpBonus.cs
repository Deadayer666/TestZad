using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpBonus : MonoBehaviour
{
    public float duration = 10f; // ������������ ������� � ��������
    public GameObject player; // ������ �� ������ ��� ���������� �������
    public Animation jumpAnimation; // �������� ������

    private bool isDoubleJumpActive = false; // ���� ���������� �������� ������
    private float remainingDuration; // ���������� ������������ �������

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