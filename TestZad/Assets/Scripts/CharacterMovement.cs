using System.Collections;
using TMPro;
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
    private bool canDoubleJump = false; // ����, ����������� ��������� ������� ������
    private bool isDoubleJumping = false; // ����, �����������, ��� ���������� ������� ������
    private Coroutine doubleJumpCoroutine; // �������� ��� ���������� �������� ������

    // ������ �� �������� UI
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI heightText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // ���������� ������ ��������
        speedText.text = "��������: " + speed;

        // ���������� ������ ������
        heightText.text = "������: " + transform.position.y;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (movement != Vector3.zero)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 desiredDirection = cameraForward * moveVertical + cameraRight * moveHorizontal;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredDirection), 0.1f);
            transform.Translate(desiredDirection * speed * Time.deltaTime, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (canDoubleJump && !isDoubleJumping)
            {
                DoubleJump();
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isDoubleJumping = false;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        StartCoroutine(Flip()); // ������������� �������� ������
        isGrounded = false;
    }

    void DoubleJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        StartCoroutine(Flip()); // ������������� �������� �������� ������
        isDoubleJumping = true;
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
        if (!hasSpeedBoost) // ����������� �������� ������ ���� ����� �� �������
        {
            float originalSpeed = speed;
            speed *= 2.0f;
            hasSpeedBoost = true;

            yield return new WaitForSeconds(duration);

            speed = originalSpeed;
            hasSpeedBoost = false;
        }
    }

    private IEnumerator Flip()
    {
        isFlipping = true;
        float flipDuration = .5f; // ������������ ������
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

        transform.rotation = startRotation; // ������� �������� � ����������� ���������
        isFlipping = false;
    }

    public void EnableDoubleJump(float duration)
    {
        if (!canDoubleJump) // �������� ������� ������ ������ ���� �� �� �������
        {
            canDoubleJump = true;
            if (doubleJumpCoroutine != null)
            {
                StopCoroutine(doubleJumpCoroutine);
            }
            doubleJumpCoroutine = StartCoroutine(DisableDoubleJumpAfter(duration));
        }
    }

    private IEnumerator DisableDoubleJumpAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        DisableDoubleJump();
    }

    public void DisableDoubleJump()
    {
        canDoubleJump = false;
        isDoubleJumping = false;
    }
}
