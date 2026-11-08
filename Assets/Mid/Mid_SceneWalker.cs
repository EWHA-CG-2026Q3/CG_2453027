using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Mid_SceneWalker : MonoBehaviour
{
    public Transform cameraTransform;
    public float moveSpeed = 2.5f;
    public float runMultiplier = 2f;
    public float lookSensitivity = 0.1f;
    public float gravity = -9.8f;
    CharacterController controller;
    float pitch;
    float verticalSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (kb == null || mouse == null)
        {
            return;
        }

        if (mouse.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (kb.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;
            transform.Rotate(0f, delta.x, 0f);
            pitch = Mathf.Clamp(pitch - delta.y, -80f, 80f);
            if (cameraTransform != null)
            {
                cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
            }
        }

        Vector3 input = Vector3.zero;
        if (kb.wKey.isPressed)
        {
            input.z += 1f;
        }

        if (kb.sKey.isPressed)
        {
            input.z -= 1f;
        }

        if (kb.dKey.isPressed)
        {
            input.x += 1f;
        }

        if (kb.aKey.isPressed)
        {
            input.x -= 1f;
        }

        if (kb.upArrowKey.isPressed)
        {
            input.z += 1f;
        }

        if (kb.downArrowKey.isPressed)
        {
            input.z -= 1f;
        }

        if (kb.rightArrowKey.isPressed)
        {
            input.x += 1f;
        }

        if (kb.leftArrowKey.isPressed)
        {
            input.x -= 1f;
        }

        float speed = moveSpeed * (kb.shiftKey.isPressed ? runMultiplier : 1f);
        Vector3 move = transform.TransformDirection(input.normalized) * speed;
        if (controller.isGrounded && verticalSpeed < 0f)
        {
            verticalSpeed = -1f;
        }

        verticalSpeed += gravity * Time.deltaTime;
        move.y = verticalSpeed;
        controller.Move(move * Time.deltaTime);
    }
}
