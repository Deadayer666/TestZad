using System.Collections;
using System.Collections.Generic;
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

    // �������� UI ��� ����������� ���������� � �������
    public GameObject[] bonusUIs; // ������ ��������, ���������� �������� UI ��� �������
    public TextMeshProUGUI[] bonusTypeTexts; // ������ ������� ��� ����������� ���� �������
    public TextMeshProUGUI[] bonusTimerTexts; // ������ ������� ��� ����������� �������� �������

    private List<BonusInfo> activeBonuses = new List<BonusInfo>();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // ������������� �������� ������� �� ���������� UI ������
        for (int i = 0; i < bonusUIs.Length; i++)
        {
            activeBonuses.Add(new BonusInfo());
        }
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
        AddBonusUI("���������", duration);
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
        float flipDuration = .3f; // ������������ ������
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
        if (doubleJumpCoroutine != null)
        {
            StopCoroutine(doubleJumpCoroutine);
        }

        doubleJumpCoroutine = StartCoroutine(DoubleJumpDuration(duration));
        AddBonusUI("������� ������", duration);
    }

    private IEnumerator DoubleJumpDuration(float duration)
    {
        canDoubleJump = true;
        yield return new WaitForSeconds(duration);
        canDoubleJump = false;
    }

    private void AddBonusUI(string bonusType, float duration)
    {
        // ����� ����� � ����� �� �����
        int existingSlot = -1;
        for (int i = 0; i < activeBonuses.Count; i++)
        {
            if (activeBonuses[i].isActive && activeBonuses[i].type == bonusType)
            {
                existingSlot = i;
                break;
            }
        }

        // ���� ����� ��� �������, �������� ��� �����������������
        if (existingSlot != -1)
        {
            activeBonuses[existingSlot].remainingTime = duration;
            activeBonuses[existingSlot].timerText.text = duration.ToString("F1");
        }
        else
        {
            // ����� ��������� ���� ��� ������ ������
            int freeSlot = -1;
            for (int i = 0; i < activeBonuses.Count; i++)
            {
                if (!activeBonuses[i].isActive)
                {
                    freeSlot = i;
                    break;
                }
            }

            // ���������, ��� ���� ������ � �� ��������� � �������� ��������
            if (freeSlot != -1 && freeSlot < bonusUIs.Length)
            {
                // ������������ ����� � �������� UI
                activeBonuses[freeSlot].Activate(bonusType, duration, bonusUIs[freeSlot], bonusTypeTexts[freeSlot], bonusTimerTexts[freeSlot]);
                StartCoroutine(UpdateBonusTimer(freeSlot));
            }
            else
            {
                Debug.LogWarning("��� ��������� ������ ��� ����������� ������� ��� �������� ����� �������� UI.");
            }
        }
    }

    private IEnumerator UpdateBonusTimer(int slotIndex)
    {
        while (activeBonuses[slotIndex].remainingTime > 0)
        {
            activeBonuses[slotIndex].remainingTime -= Time.deltaTime;
            activeBonuses[slotIndex].timerText.text = activeBonuses[slotIndex].remainingTime.ToString("F1");
            yield return null;
        }

        // ������ UI ������ ����� ��������� �������
        activeBonuses[slotIndex].Deactivate();
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

    private class BonusInfo
    {
        public bool isActive = false;
        public string type;
        public float remainingTime;
        public GameObject uiElement;
        public TextMeshProUGUI typeText;
        public TextMeshProUGUI timerText;

        public void Activate(string bonusType, float duration, GameObject ui, TextMeshProUGUI typeTxt, TextMeshProUGUI timerTxt)
        {
            type = bonusType;
            remainingTime = duration;
            uiElement = ui;
            typeText = typeTxt;
            timerText = timerTxt;

            uiElement.SetActive(true);
            typeText.text = bonusType;
            timerText.text = duration.ToString("F1");

            isActive = true;
        }

        public void Deactivate()
        {
            uiElement.SetActive(false);
            isActive = false;
        }
    }
}
