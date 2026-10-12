using UnityEngine;

public sealed class TargetCube : MonoBehaviour
{
    private Vector3 startPosition;
    private Color targetColor;

    private void Start()
    {
        startPosition = transform.position;
        targetColor = Color.HSVToRGB(Mathf.Repeat(transform.position.x * 0.11f, 1f), 0.8f, 1f);
        GetComponent<Renderer>().material.color = targetColor;
    }

    private void Update()
    {
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * 2f + startPosition.x) * 0.35f;
        transform.Rotate(0f, 55f * Time.deltaTime, 0f, Space.World);
    }

    public void Hit()
    {
        var burst = new GameObject("Target Hit VFX");
        burst.transform.position = transform.position;
        var particles = burst.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = targetColor;
        main.startLifetime = 0.45f;
        main.startSpeed = 4f;
        main.startSize = 0.16f;
        var emission = particles.emission;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 40) });
        particles.Play();
        Destroy(burst, 1f);
        Destroy(gameObject);
    }
}
