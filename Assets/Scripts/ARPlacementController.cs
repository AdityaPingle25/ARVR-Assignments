using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public sealed class ARPlacementController : MonoBehaviour
{
    [SerializeField] private Text statusText;
    private readonly List<ARRaycastHit> hits = new();
    private ARRaycastManager raycastManager;
    private int placedCount;

    private void Awake() => raycastManager = GetComponent<ARRaycastManager>();

    private void Start()
    {
        if (statusText != null) statusText.text = "Move the phone to detect a plane, then tap to place a glowing AR object.";
    }

    private void Update()
    {
        if (TryGetTapPosition(out var position)) TryPlace(position);
    }

    private bool TryGetTapPosition(out Vector2 position)
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            position = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            position = Mouse.current.position.ReadValue();
            return true;
        }

        position = default;
        return false;
    }

    private void TryPlace(Vector2 screenPosition)
    {
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            CreatePlacedObject(hits[0].pose.position, hits[0].pose.rotation);
            return;
        }

        var camera = Camera.main;
        if (camera != null && Physics.Raycast(camera.ScreenPointToRay(screenPosition), out var hit, 100f))
        {
            var forward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
            CreatePlacedObject(hit.point, forward.sqrMagnitude > 0.001f ? Quaternion.LookRotation(forward) : Quaternion.identity);
        }
        else if (statusText != null)
        {
            statusText.text = "No surface detected. Move the phone slowly and try again.";
        }
    }

    private void CreatePlacedObject(Vector3 position, Quaternion rotation)
    {
        var placed = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        placed.name = "Placed AR Object " + (++placedCount);
        placed.transform.SetPositionAndRotation(position + Vector3.up * 0.12f, rotation);
        placed.transform.localScale = new Vector3(0.28f, 0.12f, 0.28f);
        var color = Color.HSVToRGB(Mathf.Repeat(placedCount * 0.18f, 1f), 0.8f, 1f);
        placed.GetComponent<Renderer>().material.color = color;
        var light = placed.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.range = 1.6f;
        light.intensity = 1.2f;
        placed.AddComponent<PlacedARObject>().Configure(color);
        if (statusText != null) statusText.text = "Placed object " + placedCount + ". Tap another detected plane to add more.";
    }
}

public sealed class PlacedARObject : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0f, 45f * Time.deltaTime, 0f, Space.World);
    }

    public void Configure(Color color)
    {
        var particles = gameObject.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = color;
        main.startLifetime = 1.2f;
        main.startSpeed = 0.25f;
        main.startSize = 0.06f;
        var emission = particles.emission;
        emission.rateOverTime = 12f;
    }
}
