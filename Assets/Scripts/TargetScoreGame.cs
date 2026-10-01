using TMPro;
using UnityEngine;

public sealed class TargetScoreGame : MonoBehaviour
{
    [SerializeField] private Camera aimingCamera;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private int pointsPerTarget = 10;

    private int score;

    private void Start() => RefreshUI("Use the mouse button or VR trigger to fire.");

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var ray = aimingCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out var hit) && hit.collider.TryGetComponent<TargetCube>(out var target))
            RegisterHit(target);
    }

    public void RegisterHit(TargetCube target)
    {
        score += pointsPerTarget;
        target.Hit();
        RefreshUI("Target destroyed. Keep going.");
    }

    private void RefreshUI(string status)
    {
        scoreText.text = "Score: " + score;
        statusText.text = status;
    }
}
