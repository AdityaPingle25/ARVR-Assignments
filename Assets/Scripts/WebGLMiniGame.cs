using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class WebGLMiniGame : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timerText;
    [SerializeField] private float roundSeconds = 45f;

    private int score;
    private int combo;
    private float timeLeft;
    private bool playing;
    private MiniGameTarget currentTarget;

    private void Start()
    {
        gameCamera ??= Camera.main;
        StartRound();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) StartRound();
        if (!playing) return;
        timeLeft -= Time.deltaTime;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) TryHitTarget();
        if (timeLeft <= 0f) playing = false;
        UpdateHUD();
    }

    private void StartRound()
    {
        if (currentTarget != null) Destroy(currentTarget.gameObject);
        score = 0;
        combo = 0;
        timeLeft = roundSeconds;
        playing = true;
        SpawnTarget();
        UpdateHUD();
    }

    private void TryHitTarget()
    {
        if (gameCamera == null) return;
        var ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hit) && hit.collider.TryGetComponent<MiniGameTarget>(out var target))
        {
            combo++;
            score += 10 + combo * 2;
            target.Pop();
            SpawnTarget();
        }
        else combo = 0;
    }

    private void SpawnTarget()
    {
        var target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        target.name = "Glow Orb - Click Me";
        target.transform.position = new Vector3(Random.Range(-7f, 7f), Random.Range(1f, 5f), Random.Range(5f, 13f));
        target.transform.localScale = Vector3.one * Random.Range(0.65f, 1.15f);
        var color = Color.HSVToRGB(Random.value, 0.75f, 1f);
        target.GetComponent<Renderer>().material.color = color;
        var light = target.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.range = 5f;
        light.intensity = 2.5f;
        currentTarget = target.AddComponent<MiniGameTarget>();
        currentTarget.Configure(color);
    }

    private void UpdateHUD()
    {
        if (scoreText != null) scoreText.text = "Orb Pop | Score: " + score + " | Combo: x" + combo;
        if (timerText != null) timerText.text = playing
            ? "Click glowing orbs before time runs out.  R restarts.  Time: " + Mathf.CeilToInt(Mathf.Max(0f, timeLeft))
            : "Round complete. Final score: " + score + "   Press R to play again.";
    }
}

public sealed class MiniGameTarget : MonoBehaviour
{
    private Color glowColor;
    private Vector3 basePosition;

    public void Configure(Color color)
    {
        glowColor = color;
        basePosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(35f * Time.deltaTime, 75f * Time.deltaTime, 0f, Space.World);
        transform.position = basePosition + Vector3.up * Mathf.Sin(Time.time * 3f + basePosition.x) * 0.35f;
    }

    public void Pop()
    {
        var burst = new GameObject("Orb Pop VFX");
        burst.transform.position = transform.position;
        var particles = burst.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = glowColor;
        main.startLifetime = 0.65f;
        main.startSpeed = 4.5f;
        main.startSize = 0.18f;
        main.maxParticles = 60;
        var emission = particles.emission;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 32) });
        particles.Play();
        Destroy(burst, 1.2f);
        Destroy(gameObject);
    }
}
