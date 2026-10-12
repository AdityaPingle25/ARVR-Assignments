using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float walkSpeed = 4f, jumpHeight = 1.2f, gravity = -18f, mouseSensitivity = 0.12f, crouchHeight = 1.1f;
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0f, 1.7f, -4.5f);
    private CharacterController controller;
    private float verticalVelocity, pitch, standingHeight;
    private bool thirdPerson;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCamera ??= Camera.main;
        standingHeight = controller.height;

        var capsuleRenderer = GetComponent<MeshRenderer>();
        if (capsuleRenderer != null) capsuleRenderer.enabled = false;
    }
    private void Start() { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; CreateLandmarks(); }

    private void Update()
    {
        var keyboard = Keyboard.current; var mouse = Mouse.current;
        if (keyboard == null || playerCamera == null) return;
        if (keyboard.escapeKey.wasPressedThisFrame) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        if (keyboard.vKey.wasPressedThisFrame) thirdPerson = !thirdPerson;
        if (Cursor.lockState == CursorLockMode.Locked && mouse != null) { var delta = mouse.delta.ReadValue() * mouseSensitivity; pitch = Mathf.Clamp(pitch - delta.y, -80f, 80f); transform.Rotate(Vector3.up * delta.x); playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f); }
        var crouching = keyboard.cKey.isPressed || keyboard.leftCtrlKey.isPressed;
        controller.height = crouching ? crouchHeight : standingHeight;
        controller.center = Vector3.down * (standingHeight - controller.height) * 0.5f;
        playerCamera.transform.localPosition = thirdPerson
            ? new Vector3(thirdPersonOffset.x, crouching ? 1.25f : thirdPersonOffset.y, thirdPersonOffset.z)
            : Vector3.up * (controller.height * 0.45f);
        var horizontal = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
        var vertical = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);
        controller.Move((transform.forward * vertical + transform.right * horizontal).normalized * walkSpeed * Time.deltaTime);
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -1f;
        if (controller.isGrounded && keyboard.spaceKey.wasPressedThisFrame) verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        verticalVelocity += gravity * Time.deltaTime; controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void CreateLandmarks() { CreateBlock("Blue - Left", new Vector3(-3f, .5f, 4f), Color.blue); CreateBlock("Yellow - Right", new Vector3(3f, .5f, 4f), Color.yellow); CreateBlock("Red - Ahead", new Vector3(0f, .5f, 7f), Color.red); CreateBlock("Green - Far", new Vector3(0f, .5f, 12f), Color.green); }
    private static void CreateBlock(string label, Vector3 position, Color color) { var block = GameObject.CreatePrimitive(PrimitiveType.Cube); block.name = label; block.transform.position = position; block.GetComponent<Renderer>().material.color = color; block.AddComponent<InteractiveObject>(); }
}
