using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookController : MonoBehaviour
{
    [SerializeField]
    float mouseSensitivity = 100.0f;
    Transform characterBody;

    private void Awake()
    {
        characterBody = transform.parent;
        if (characterBody.name != "Character")
            throw new System.Exception("My parent must be a Character !!");
    }
    float xRotation = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        characterBody.Rotate(Vector3.up * mouseX);
    }
}
