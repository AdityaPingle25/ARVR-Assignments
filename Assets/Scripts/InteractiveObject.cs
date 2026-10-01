using UnityEngine;

public sealed class InteractiveObject : MonoBehaviour
{
    [SerializeField] private Color activeColor = new Color(0.2f, 0.95f, 0.7f);
    private Renderer cachedRenderer;
    private bool active;

    private void Awake() => cachedRenderer = GetComponent<Renderer>();

    public void Interact()
    {
        active = !active;
        cachedRenderer.material.color = active ? activeColor : Color.white;
        transform.Rotate(0f, 45f, 0f, Space.World);
    }
}
