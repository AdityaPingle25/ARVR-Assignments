using TMPro;
using UnityEngine;

public sealed class WebGLMiniGame : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float roundSeconds = 45f;

    private int score;
    private float timeLeft;
    private bool playing;

    private void Start()
    {
        timeLeft = roundSeconds;
        playing = true;
        SpawnTarget();
        UpdateHUD();
    }

    private void Update()
    {
        if (!playing) return;
        timeLeft -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0)) TryHitTarget();
        if (timeLeft <= 0f) playing = false;
        UpdateHUD();
    }

    private void TryHitTarget()
    {
        var ray = gameCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit) && hit.collider.TryGetComponent<MiniGameTarget>(out var target))
        {
            score += 5;
            target.Pop();
            SpawnTarget();
        }
    }

    private void SpawnTarget()
    {
        var target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        target.name = "Click Target";
        target.transform.position = new Vector3(Random.Range(-7f, 7f), Random.Range(0.5f, 4f), Random.Range(5f, 11f));
        target.GetComponent<Renderer>().material.color = new Color(Random.value, 0.7f, 1f);
        target.AddComponent<MiniGameTarget>();
    }

    private void UpdateHUD()
    {
        scoreText.text = "Score: " + score;
        timerText.text = playing ? "Time: " + Mathf.CeilToInt(Mathf.Max(0f, timeLeft)) : "Round complete - Score: " + score;
    }
}

public sealed class MiniGameTarget : MonoBehaviour
{
    public void Pop()
    {
        var particles = gameObject.AddComponent<ParticleSystem>();
        particles.Play();
        Destroy(gameObject, 0.15f);
    }
}
