using UnityEngine;
using TMPro;
using Vuforia;

public class VuforiaStatusDisplay : MonoBehaviour
{
    [SerializeField] private ObserverBehaviour observerBehaviour;
    [SerializeField] private TMP_Text displayText;

    private void Reset()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
    }

    private void OnEnable()
    {
        if (observerBehaviour == null)
            observerBehaviour = GetComponent<ObserverBehaviour>();

        if (observerBehaviour != null)
            observerBehaviour.OnTargetStatusChanged += OnStatusChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (observerBehaviour != null)
            observerBehaviour.OnTargetStatusChanged -= OnStatusChanged;
    }

    private void Update() => Refresh();

    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        Refresh();
        Debug.Log($"[Vuforia] {behaviour.TargetName} | {status.Status} | {status.StatusInfo}");
    }

    private void Refresh()
    {
        if (displayText == null || observerBehaviour == null)
            return;

        TargetStatus ts = observerBehaviour.TargetStatus;
        bool poseValid = ts.Status == Status.TRACKED || ts.Status == Status.EXTENDED_TRACKED;

        displayText.text =
            $"Target: {observerBehaviour.TargetName}\n" +
            $"Status: {ts.Status}\n" +
            $"StatusInfo: {ts.StatusInfo}\n" +
            $"Pose valid: {poseValid}";
    }
}