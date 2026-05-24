using UnityEngine;
using Vuforia;
using TMPro;

public class DetectedModelUI : MonoBehaviour
{
    public TextMeshPro text;
    ObserverBehaviour observer;
    public bool detectionEnabled = false;

    void Awake()
    {
        observer = GetComponent<ObserverBehaviour>();
        if (observer != null)
        {
            observer.OnTargetStatusChanged += OnStatusChanged;
        }
    }

    void OnDestroy()
    {
        if (observer != null)
        {
            observer.OnTargetStatusChanged -= OnStatusChanged;
        }
    }

    public void EnableDetection()
    {
        detectionEnabled = true;

        if (text != null)
            text.text = "Detection ON";
    }

    public void DisableDetection()
    {
        detectionEnabled = false;

        if (text != null)
            text.text = "Detection OFF";
    }

    void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (!detectionEnabled)
            return;

        if (status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED)
        {
            text.text = "Detected: " + behaviour.TargetName;
        }
        else if (status.Status == Status.NO_POSE)
        {
            text.text = "Detected: ---";
        }
    }
}
