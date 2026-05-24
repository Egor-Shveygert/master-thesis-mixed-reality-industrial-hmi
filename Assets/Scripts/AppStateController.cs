using UnityEngine;
using Vuforia;

public class AppStateController : MonoBehaviour
{
    [Header("Detection")]
    public ModelTargetBehaviour modelTargetBehaviour;
    public DetectedModelLogger detectedLogger;
    public DetectedModelUI detectedUI;
    public GameObject detectionMenu;

    [Header("UI after detection")]
    public GameObject variablesUIRoot;

    [Header("Guide View Renderer Position")]
    public Vector3 guideRendererLocalPosition = new Vector3(0f, 0f, 0.1f);

    private bool detectionStarted = false;
    private bool isDetected = false;

    void Start()
    {
        if (modelTargetBehaviour != null)
            modelTargetBehaviour.GuideViewMode = ModelTargetBehaviour.GuideViewDisplayMode.NoGuideView;

        if (detectedLogger != null)
            detectedLogger.DisableDetection();

        if (detectedUI != null)
            detectedUI.DisableDetection();

        if (detectionMenu != null)
            detectionMenu.SetActive(true);

        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(false);

        Debug.Log("App initialized.");
    }

    public void StartDetection()
    {
        detectionStarted = true;
        isDetected = false;

        if (modelTargetBehaviour != null)
            modelTargetBehaviour.GuideViewMode = ModelTargetBehaviour.GuideViewDisplayMode.GuideView2D;

        if (detectedLogger != null)
            detectedLogger.EnableDetection();

        if (detectedUI != null)
            detectedUI.EnableDetection();

        if (detectionMenu != null)
            detectionMenu.SetActive(true);

        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(false);

        Invoke(nameof(AdjustGuideRendererPosition), 0.2f);

        Debug.Log("Detection started.");
    }

    void AdjustGuideRendererPosition()
    {
        GameObject guideRenderer = GameObject.Find("GuideViewRenderer");

        if (guideRenderer == null)
        {
            Debug.LogWarning("GuideViewRenderer nebol najdeny.");
            return;
        }

        guideRenderer.transform.localPosition = guideRendererLocalPosition;

        Debug.Log("GuideViewRenderer position upravena.");
    }

    public void ConfirmDetectionSuccess()
    {
        if (!detectionStarted || isDetected)
            return;

        isDetected = true;

        if (detectedLogger != null)
            detectedLogger.DisableDetection();

        if (detectedUI != null)
            detectedUI.DisableDetection();

        if (modelTargetBehaviour != null)
            modelTargetBehaviour.GuideViewMode = ModelTargetBehaviour.GuideViewDisplayMode.NoGuideView;

        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(true);

        Debug.Log("Detection success confirmed. Variables UI shown.");
    }

    public void ToggleVariablesUI()
    {
        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(!variablesUIRoot.activeSelf);
    }

    public void ShowVariablesUI()
    {
        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(true);
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

        if (modelTargetBehaviour != null)
            modelTargetBehaviour.GuideViewMode = ModelTargetBehaviour.GuideViewDisplayMode.NoGuideView;

        if (detectedLogger != null)
            detectedLogger.DisableDetection();

        if (detectedUI != null)
            detectedUI.DisableDetection();

        if (detectionMenu != null)
            detectionMenu.SetActive(true);

        if (variablesUIRoot != null)
            variablesUIRoot.SetActive(false);

        Debug.Log("Detection state reset.");
    }
}
