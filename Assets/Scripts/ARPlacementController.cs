using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public sealed class ARPlacementController : MonoBehaviour
{
    [SerializeField] private GameObject placementPrefab;
    private readonly List<ARRaycastHit> hits = new();
    private ARRaycastManager raycastManager;

    private void Awake() => raycastManager = GetComponent<ARRaycastManager>();

    private void Update()
    {
        if (Input.touchCount == 0) return;
        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began || !raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon)) return;
        Instantiate(placementPrefab, hits[0].pose.position, hits[0].pose.rotation);
    }
}
