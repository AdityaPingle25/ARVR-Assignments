using UnityEngine;

public sealed class TargetCube : MonoBehaviour
{
    public void Hit()
    {
        var burst = new GameObject("Target Hit VFX");
        burst.transform.position = transform.position;
        var particles = burst.AddComponent<ParticleSystem>();
        var main = particles.main;
        main.startColor = Color.cyan;
        main.startLifetime = 0.45f;
        main.startSpeed = 3f;
        particles.Play();
        Destroy(burst, 1f);
        Destroy(gameObject);
    }
}
