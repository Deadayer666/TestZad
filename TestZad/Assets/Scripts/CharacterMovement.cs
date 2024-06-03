using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpForce = 8.0f;
    private Rigidbody rb;
    private bool isGrounded;
    private bool hasSpeedBoost = false;
    private Coroutine speedBoostCoroutine;
    private bool isFlipping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (movement != Vector3.zero)
        {
            // Преобразование ввода движения в глобальную систему координат относительно камеры
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // Игнорируем компонент высоты направления камеры
            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 desiredDirection = cameraForward * moveVertical + cameraRight * moveHorizontal;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredDirection), 0.1f);
            transform.Translate(desiredDirection * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isFlipping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            StartCoroutine(Flip());
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void ApplySpeedBoost(float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoost(duration));
    }

    private IEnumerator SpeedBoost(float duration)
    {
        float originalSpeed = speed;
        speed *= 2.0f;
        hasSpeedBoost = true;

        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
        hasSpeedBoost = false;
    }

    private IEnumerator Flip()
    {
        isFlipping = true;
        float flipDuration = .5f; // Длительность сальто
        float elapsedTime = 0f;

        Quaternion startRotation = transform.rotation;

        while (elapsedTime < flipDuration)
        {
            float flipProgress = elapsedTime / flipDuration;
            float currentAngle = Mathf.Lerp(0, 360, flipProgress);

            transform.rotation = startRotation * Quaternion.Euler(currentAngle, 0, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = startRotation; // Вернуть вращение к нормальному состоянию
        isFlipping = false;
    }
}