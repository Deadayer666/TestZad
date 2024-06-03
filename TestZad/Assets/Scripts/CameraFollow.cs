using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float rotationSpeed = 5.0f;

    private Vector3 offset;
    private float currentYaw;
    private float currentPitch;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
        float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;

        currentYaw += horizontal;
        currentPitch -= vertical;
        currentPitch = Mathf.Clamp(currentPitch, -45f, 45f);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        transform.position = player.transform.position + rotation * offset;
        transform.LookAt(player.transform.position);
    }
}
