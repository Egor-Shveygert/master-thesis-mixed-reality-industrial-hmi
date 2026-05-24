using UnityEngine;
using Vuforia;

public class DetectedModelLogger : MonoBehaviour
{
    private ObserverBehaviour observer;

    [Header("Detection")]
    public bool detectionEnabled = false;

    [Header("App flow")]
    public AMTargetAppStateController appStateController;

    private bool trackedAlreadyTriggered = false;

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
        trackedAlreadyTriggered = false;

        Debug.Log("Detection enabled for: " + gameObject.name);
    }

    public void DisableDetection()
    {
        detectionEnabled = false;

        Debug.Log("Detection disabled for: " + gameObject.name);
    }

    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (!detectionEnabled)
            return;

        string targetName = behaviour.TargetName;
        Status currentStatus = status.Status;

        Debug.Log("Model Target status changed: " + targetName + " -> " + currentStatus);

        if (appStateController != null)
        {
            appStateController.OnModelTargetStatusChanged(targetName, currentStatus);
        }
        else
        {
            Debug.LogWarning("AMTargetAppStateController reference is missing in DetectedModelLogger.");
        }

        if (currentStatus == Status.TRACKED)
        {
            if (!trackedAlreadyTriggered)
            {
                trackedAlreadyTriggered = true;
            }
        }
        else if (currentStatus == Status.NO_POSE)
        {
            trackedAlreadyTriggered = false;
        }
    }
}
