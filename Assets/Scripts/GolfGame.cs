using TMPro;
using UnityEngine;

public sealed class GolfGame : MonoBehaviour
{
    [SerializeField] private TMP_Text scorecardText;
    private int strokes;
    private int holesCompleted;

    public void RegisterStroke()
    {
        strokes++;
        RefreshScorecard();
    }

    public void CompleteHole(int par)
    {
        holesCompleted++;
        RefreshScorecard("Hole " + holesCompleted + " complete: " + strokes + " strokes, par " + par);
        strokes = 0;
    }

    private void Start() => RefreshScorecard("Hit the ball toward a cup.");

    private void RefreshScorecard(string message = "")
    {
        scorecardText.text = "Golf scorecard\nHoles completed: " + holesCompleted + "\nCurrent strokes: " + strokes + "\n" + message;
    }
}

public sealed class GolfBall : MonoBehaviour
{
    [SerializeField] private GolfGame game;
    [SerializeField] private float hitForce = 7f;
    private Rigidbody body;

    private void Awake() => body = GetComponent<Rigidbody>();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && body.velocity.sqrMagnitude < 0.05f)
        {
            body.AddForce(Camera.main.transform.forward * hitForce + Vector3.up, ForceMode.Impulse);
            game.RegisterStroke();
        }
    }
}

public sealed class GolfHole : MonoBehaviour
{
    [SerializeField] private GolfGame game;
    [SerializeField] private int par = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<GolfBall>(out _)) return;
        game.CompleteHole(par);
        other.transform.position = transform.position + Vector3.back * 2f + Vector3.up;
        other.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
