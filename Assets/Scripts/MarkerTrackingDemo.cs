using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class MarkerTrackingDemo : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private MarkerTrackedObject marker;
    [SerializeField] private Text statusText;
    private bool tracked;

    private void Start()
    {
        sceneCamera ??= Camera.main;
        SetTracked(true);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame) SetTracked(!tracked);
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && sceneCamera != null)
        {
            var ray = sceneCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == gameObject) SetTracked(!tracked);
        }
    }

    private void SetTracked(bool value)
    {
        tracked = value;
        marker.SetTracking(tracked);
        if (statusText != null)
            statusText.text = tracked
                ? "Marker detected: tracked content is visible. Press M or click the marker to simulate tracking loss."
                : "Marker lost: press M or click the marker to simulate detection.";
    }
}

public sealed class TrackedContentAnimator : MonoBehaviour
{
    private Vector3 baseScale;

    private void Start() => baseScale = transform.localScale;

    private void Update()
    {
        transform.Rotate(0f, 55f * Time.deltaTime, 0f, Space.World);
        transform.localScale = baseScale * (1f + Mathf.Sin(Time.time * 3f) * 0.08f);
    }
}
