using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class GolfGame : MonoBehaviour
{
    [SerializeField] private Text scorecardText;
    private GolfHole[] holes;
    private GolfBall ball;
    private int strokes;
    private int totalStrokes;
    private int holeIndex;

    private void Start()
    {
        holes = FindObjectsOfType<GolfHole>().OrderBy(hole => hole.HoleNumber).ToArray();
        ball = FindObjectOfType<GolfBall>();
        for (var i = 0; i < holes.Length; i++) holes[i].gameObject.SetActive(i == 0);
        PlaceBallAtTee();
        RefreshScorecard("Hold and release the left mouse button to hit the ball.");
    }

    public void RegisterStroke()
    {
        strokes++;
        totalStrokes++;
        RefreshScorecard("Nice swing. Guide the ball into the glowing cup.");
    }

    public void CompleteHole(GolfHole hole)
    {
        if (holeIndex >= holes.Length || holes[holeIndex] != hole) return;
        hole.gameObject.SetActive(false);
        var completedHole = holeIndex + 1;
        holeIndex++;
        if (holeIndex >= holes.Length)
        {
            RefreshScorecard("Course complete. " + totalStrokes + " total strokes. Press R to restart.");
            return;
        }

        holes[holeIndex].gameObject.SetActive(true);
        strokes = 0;
        PlaceBallAtTee();
        RefreshScorecard("Hole " + completedHole + " complete. Next tee is ready.");
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame && holeIndex >= holes.Length)
        {
            holeIndex = 0;
            strokes = 0;
            totalStrokes = 0;
            for (var i = 0; i < holes.Length; i++) holes[i].gameObject.SetActive(i == 0);
            PlaceBallAtTee();
            RefreshScorecard("New round started.");
        }
    }

    private void PlaceBallAtTee()
    {
        if (ball == null || holeIndex >= holes.Length) return;
        ball.ResetBall(holes[holeIndex].transform.position - Vector3.forward * 6f + Vector3.up * 0.5f);
    }

    private void RefreshScorecard(string message = "")
    {
        if (scorecardText == null) return;
        var currentHole = Mathf.Min(holeIndex + 1, holes?.Length ?? 3);
        scorecardText.text = "VR Golf | Hole " + currentHole + "/" + (holes?.Length ?? 3) + "\n" +
                              "Hole strokes: " + strokes + " | Total: " + totalStrokes + "\n" + message;
    }
}

public sealed class GolfBall : MonoBehaviour
{
    [SerializeField] private GolfGame game;
    [SerializeField] private float minHitForce = 4f;
    [SerializeField] private float maxHitForce = 14f;
    private Rigidbody body;
    private LineRenderer aimLine;
    private float charge;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        aimLine = gameObject.AddComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.startWidth = 0.04f;
        aimLine.endWidth = 0.01f;
        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.startColor = Color.yellow;
        aimLine.endColor = Color.red;
    }

    private void Update()
    {
        var canHit = body.velocity.sqrMagnitude < 0.03f;
        if (Mouse.current != null && canHit && Mouse.current.leftButton.isPressed)
            charge = Mathf.Clamp01(charge + Time.deltaTime);

        if (Mouse.current != null && canHit && Mouse.current.leftButton.wasReleasedThisFrame && charge > 0.05f)
        {
            var direction = Camera.main == null ? Vector3.forward : Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            body.AddForce(direction * Mathf.Lerp(minHitForce, maxHitForce, charge) + Vector3.up * 1.2f, ForceMode.Impulse);
            charge = 0f;
            game.RegisterStroke();
        }

        UpdateAimLine(canHit);
    }

    public void ResetBall(Vector3 position)
    {
        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        transform.position = position;
        charge = 0f;
    }

    private void UpdateAimLine(bool canHit)
    {
        aimLine.enabled = canHit;
        if (!canHit) return;
        var direction = Camera.main == null ? Vector3.forward : Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        aimLine.SetPosition(0, transform.position + Vector3.up * 0.08f);
        aimLine.SetPosition(1, transform.position + direction * Mathf.Lerp(1.5f, 5f, charge) + Vector3.up * 0.08f);
    }
}

public sealed class GolfHole : MonoBehaviour
{
    [SerializeField] private GolfGame game;
    [SerializeField] private int holeNumber;
    public int HoleNumber => holeNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<GolfBall>(out _)) game.CompleteHole(this);
    }
}

public sealed class GolfCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform ball;

    private void LateUpdate()
    {
        if (ball == null) return;
        transform.position = Vector3.Lerp(transform.position, ball.position + new Vector3(0f, 5f, -10f), Time.deltaTime * 3f);
        transform.LookAt(ball.position + Vector3.up * 0.3f);
    }
}
