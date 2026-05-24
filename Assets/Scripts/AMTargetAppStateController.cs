using System.Collections;
using TMPro;
using UnityEngine;
using Vuforia;

public class AMTargetAppStateController : MonoBehaviour
{
    [Header("Multiple Model Targets")]
    [Tooltip("Assign all GameObjects that contain ModelTargetBehaviour components.")]
    public GameObject[] modelTargetObjects;
    [Tooltip("Assign all DetectedModelLogger components from the Model Target objects.")]
    public DetectedModelLogger[] detectedLoggers;
    [Tooltip("How long the Model Target GameObjects stay disabled during reset.")]
    public float modelTargetRestartDelay = 0.2f;
    [Header("Symbolic Guide View")]
    [Tooltip("Common symbolic guide view shown while the application is searching for a 3D identifier.")]
    public GameObject symbolicGuideViewRoot;
    [Header("UI after detection")]
    public GameObject variablesUIRoot;
    [Header("Detection UI Texts")]
    public TMP_Text detectionStateText;
    public TMP_Text detectedTargetNameText;
    public TMP_Text targetTrackingStatusText;
    [Header("Detection UI Colors")]
    public Color detectionOffColor = Color.gray;
    public Color detectionOnColor = Color.white;
    public Color detectionSuccessfulColor = Color.green;
    public Color uiModeColor = Color.cyan;
    [Header("Messages")]
    [Tooltip("How long the Detection Successful message is shown before switching to UI Mode.")]
    public float successfulMessageDuration = 3.0f;
    private bool detectionStarted = false;
    private bool isDetected = false;
    private string currentDetectedTargetName = "---";
    private string currentTargetStatus = "NO_POSE";
    private Coroutine restartModelTargetsCoroutine;
    private Coroutine detectionSuccessfulCoroutine;

    void Start()
    {
        detectionStarted = false;
        isDetected = false;
        DisableAllDetectionLoggers();

        if (symbolicGuideViewRoot != null)
            symbolicGuideViewRoot.SetActive(false);

        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(false);

        SetDetectionStateText("Detection OFF", detectionOffColor);
        SetDetectedTargetName("---");
        SetTargetTrackingStatus("NO_POSE");
        Debug.Log("Advanced Model Target app initialized.");
    }

    public void StartDetection()
    {
        detectionStarted = true;
        isDetected = false;
        currentDetectedTargetName = "---";
        currentTargetStatus = "NO_POSE";
        SetModelTargetsActive(true);
        EnableAllDetectionLoggers();

        if (symbolicGuideViewRoot != null)
            symbolicGuideViewRoot.SetActive(true);

        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(false);

        SetDetectionStateText("Detection ON", detectionOnColor);
        SetDetectedTargetName("---");
        SetTargetTrackingStatus("NO_POSE");
        Debug.Log("Detection started.");
    }

    public void ConfirmDetectionSuccess()
    {
        ConfirmDetectionSuccess("Manual confirmation", "MANUAL", true);
    }
    private void ConfirmDetectionSuccess(string targetName, string targetStatus, bool showManualVariables)
    {
        if (!detectionStarted || isDetected)
            return;

        isDetected = true;
        currentDetectedTargetName = targetName;
        currentTargetStatus = targetStatus;

        DisableAllDetectionLoggers();

        if (symbolicGuideViewRoot != null)
            symbolicGuideViewRoot.SetActive(false);

        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(showManualVariables);

        SetDetectedTargetName(currentDetectedTargetName);
        SetTargetTrackingStatus(currentTargetStatus);

        if (detectionSuccessfulCoroutine != null)
            StopCoroutine(detectionSuccessfulCoroutine);

        detectionSuccessfulCoroutine = StartCoroutine(ShowDetectionSuccessfulThenUIMode());

        Debug.Log("Detection success confirmed. Target: " + targetName + ", Status: " + targetStatus + ", Manual variables: " + showManualVariables);
    }

    private IEnumerator ShowDetectionSuccessfulThenUIMode()
    {
        SetDetectionStateText("Detection Successful", detectionSuccessfulColor);

        yield return new WaitForSeconds(successfulMessageDuration);

        if (isDetected)
        {
            SetDetectionStateText("UI Mode", uiModeColor);
        }
    }

    public void OnModelTargetStatusChanged(string targetName, Status status)
    {
        string statusText = status.ToString();
        SetTargetTrackingStatus(statusText);

        if (!detectionStarted || isDetected)
            return;

        if (status == Status.TRACKED)
        {
            SetDetectedTargetName(targetName);
            ConfirmDetectionSuccess(targetName, statusText, false);
        }
        else if (status == Status.EXTENDED_TRACKED)
        {
            SetDetectedTargetName(targetName);
            Debug.Log("EXTENDED_TRACKED ignored as detection trigger: " + targetName);
        }
        else if (status == Status.NO_POSE)
        {
            SetDetectedTargetName("---");
        }
    }

    public void ToggleVariablesUI()
    {
        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(!variablesUIRoot.activeSelf);

        if (variablesUIRoot != null && variablesUIRoot.activeSelf)
            SetDetectionStateText("UI Mode", uiModeColor);
    }

    public void ShowVariablesUI()
    {
        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(true);

        if (symbolicGuideViewRoot != null)
            symbolicGuideViewRoot.SetActive(false);

        SetDetectionStateText("UI Mode", uiModeColor);
    }

    public void HideVariablesUI()
    {
        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(false);
    }

    public void ResetDetectionState()
    {
        detectionStarted = false;
        isDetected = false;
        currentDetectedTargetName = "---";
        currentTargetStatus = "NO_POSE";

        if (detectionSuccessfulCoroutine != null)
        {
            StopCoroutine(detectionSuccessfulCoroutine);
            detectionSuccessfulCoroutine = null;
        }

        DisableAllDetectionLoggers();
        if (symbolicGuideViewRoot != null)
            symbolicGuideViewRoot.SetActive(false);

        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(false);

        SetDetectionStateText("Detection OFF", detectionOffColor);
        SetDetectedTargetName("---");
        SetTargetTrackingStatus("NO_POSE");

        if (restartModelTargetsCoroutine != null)
            StopCoroutine(restartModelTargetsCoroutine);

        restartModelTargetsCoroutine = StartCoroutine(RestartModelTargetsRoutine());

        Debug.Log("Detection state reset.");
    }

    private IEnumerator RestartModelTargetsRoutine()
    {
        SetModelTargetsActive(false);
        yield return new WaitForSeconds(modelTargetRestartDelay);
        SetModelTargetsActive(true);
        Debug.Log("Model Target GameObjects restarted.");
    }

    private void EnableAllDetectionLoggers()
    {
        if (detectedLoggers == null)
            return;

        foreach (DetectedModelLogger logger in detectedLoggers)
        {
            if (logger != null)
                logger.EnableDetection();
        }
    }

    private void DisableAllDetectionLoggers()
    {
        if (detectedLoggers == null)
            return;

        foreach (DetectedModelLogger logger in detectedLoggers)
        {
            if (logger != null)
                logger.DisableDetection();
        }
    }

    private void SetModelTargetsActive(bool active)
    {
        if (modelTargetObjects == null)
            return;

        foreach (GameObject targetObject in modelTargetObjects)
        {
            if (targetObject != null)
                targetObject.SetActive(active);
        }
    }

    private void SetDetectionStateText(string text, Color color)
    {
        if (detectionStateText == null)
            return;

        detectionStateText.text = text;
        detectionStateText.color = color;
    }

    private void SetDetectedTargetName(string targetName)
    {
        if (detectedTargetNameText == null)
            return;

        detectedTargetNameText.text = targetName;
    }

    private void SetTargetTrackingStatus(string status)
    {
        if (targetTrackingStatusText == null)
            return;

        targetTrackingStatusText.text = status;
    }
}