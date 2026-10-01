using UnityEngine;

public sealed class ObjectInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float reach = 5f;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        var ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out var hit, reach) && hit.collider.TryGetComponent<InteractiveObject>(out var interactive))
            interactive.Interact();
    }
}
