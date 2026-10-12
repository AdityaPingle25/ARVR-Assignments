using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ObjectInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float reach = 5f;
    [SerializeField] private float holdDistance = 2f;
    private Rigidbody heldBody;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    private void Update()
    {
        if (playerCamera == null) return;
        if (heldBody != null)
        {
            heldBody.transform.position = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
            heldBody.transform.rotation = playerCamera.transform.rotation;
        }
        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame) return;
        if (heldBody != null) { heldBody.isKinematic = false; heldBody.useGravity = true; heldBody = null; return; }
        var ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out var hit, reach) && hit.collider.TryGetComponent<InteractiveObject>(out var interactive))
        {
            interactive.Interact();
            heldBody = hit.rigidbody ?? hit.collider.gameObject.AddComponent<Rigidbody>();
            heldBody.useGravity = false;
            heldBody.isKinematic = true;
        }
    }
}
