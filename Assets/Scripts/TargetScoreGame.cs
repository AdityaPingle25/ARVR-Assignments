using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using XRInputDevice = UnityEngine.XR.InputDevice;

public sealed class TargetScoreGame : MonoBehaviour
{
    [SerializeField] private Camera aimingCamera;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text statusText;
    [SerializeField] private int pointsPerTarget = 10;

    private readonly List<XRInputDevice> controllers = new();
    private int score;
    private int targetsRemaining;
    private bool triggerHeld;

    private void Start()
    {
        aimingCamera ??= Camera.main;
        targetsRemaining = FindObjectsOfType<TargetCube>().Length;
        RefreshUI("Aim with the centre reticle. Click, gamepad trigger, or VR trigger to fire.");
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Fire(Mouse.current.position.ReadValue());
            return;
        }

        if (WasControllerTriggerPressedThisFrame())
            Fire(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
    }

    private bool WasControllerTriggerPressedThisFrame()
    {
        if (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame) return true;

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, controllers);
        var pressed = false;
        foreach (var controller in controllers)
        {
            if (controller.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out var value) && value)
            {
                pressed = true;
                break;
            }
        }

        var pressedThisFrame = pressed && !triggerHeld;
        triggerHeld = pressed;
        return pressedThisFrame;
    }

    private void Fire(Vector2 screenPosition)
    {
        if (aimingCamera == null || targetsRemaining == 0) return;
        var ray = aimingCamera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out var hit, 100f) && hit.collider.TryGetComponent<TargetCube>(out var target))
            RegisterHit(target);
        else
            RefreshUI("Missed. Keep the reticle on a target cube.");
    }

    public void RegisterHit(TargetCube target)
    {
        score += pointsPerTarget;
        targetsRemaining--;
        target.Hit();
        RefreshUI(targetsRemaining > 0
            ? "Direct hit. " + targetsRemaining + " cubes remaining."
            : "Range complete. Final score: " + score);
    }

    private void RefreshUI(string status)
    {
        if (scoreText != null) scoreText.text = "VR Target Range | Score: " + score + " | Targets: " + targetsRemaining;
        if (statusText != null) statusText.text = status;
    }
}
