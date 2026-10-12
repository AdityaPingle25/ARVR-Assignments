using UnityEngine;

public sealed class MarkerTrackedObject : MonoBehaviour
{
    [SerializeField] private GameObject trackedContent;

    private void Awake()
    {
        if (trackedContent != null) trackedContent.SetActive(false);
    }

    // Assign this from Vuforia's Observer Event Handler after importing the Vuforia Engine package.
    public void SetTracking(bool isTracked)
    {
        if (trackedContent != null) trackedContent.SetActive(isTracked);
    }
}
