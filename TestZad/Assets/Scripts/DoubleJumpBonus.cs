using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpBonus : MonoBehaviour
{
    public float rotationSpeed = 50.0f;
    public float duration = 10f;
    private bool isCollected = false;
    private Vector3 originalPosition; // ������ ��������� ������� ������
    private Coroutine respawnCoroutine; // �������� ��� �������� ������

    public GameObject player;
    private CharacterMovement characterMovement;

    private void Start()
    {
        if (player != null)
        {
            characterMovement = player.GetComponent<CharacterMovement>();
        }
        originalPosition = transform.position;
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
                StartCoroutine(CollectEffect());
            }
        }
    }

    IEnumerator CollectEffect()
    {
        isCollected = true;
        float disappearDuration = 1.0f;

        for (float t = 0; t < disappearDuration; t += Time.deltaTime)
        {
            transform.position = originalPosition + Vector3.up * (t / disappearDuration);
            transform.localScale = Vector3.one * (1 - t / disappearDuration);
            yield return null;
        }

        // �������������� ����� ����� ���������� ��������
        gameObject.SetActive(false);
        respawnCoroutine = StartCoroutine(RespawnAfterDelay(5.0f));
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // �������� ����� � �������� �������
        gameObject.SetActive(true);
        transform.position = originalPosition;
        isCollected = false;
        // �������� �������� ��������
        respawnCoroutine = null;
    }

    // ������ �������� ��� ���������� �������
    void OnDestroy()
    {
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }
    }
}