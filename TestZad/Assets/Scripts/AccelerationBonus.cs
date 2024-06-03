using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationBonus : MonoBehaviour
{
    public float rotationSpeed = 50.0f;
    public float boostDuration = 10.0f;
    private bool isCollected = false;

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
                StartCoroutine(CollectEffect());
            }
        }
    }

    IEnumerator CollectEffect()
    {
        isCollected = true;
        float disappearDuration = 1.0f;
        Vector3 originalPosition = transform.position;

        for (float t = 0; t < disappearDuration; t += Time.deltaTime)
        {
            transform.position = originalPosition + Vector3.up * (t / disappearDuration);
            transform.localScale = Vector3.one * (1 - t / disappearDuration);
            yield return null;
        }

        Destroy(gameObject);
    }
}