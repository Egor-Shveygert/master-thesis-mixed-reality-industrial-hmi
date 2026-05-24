//using System;
//using System.Collections;
//using UnityEngine;
//using Vuforia;

//public class AMTargetAppStateController : MonoBehaviour
//{
//    [Header("Detection")]
//    public ModelTargetBehaviour modelTargetBehaviour;
//    public DetectedModelLogger detectedLogger;
//    public DetectedModelUI detectedUI;
//    public GameObject detectionMenu;

//    [Header("Model Target Objects")]
//    [Tooltip("Sem pridaj GameObjecty, ktore obsahuju ModelTargetBehaviour. Napr. ModelTarget, ModelTarget (1), ...")]
//    public GameObject[] modelTargetObjects;

//    [Tooltip("Ako dlho ostanu ModelTarget objekty vypnute pri resete.")]
//    public float modelTargetRestartDelay = 0.2f;

//    [Header("UI after detection")]
//    public GameObject variablesUIRoot;

//    [Header("Guide View Renderer Position")]
//    public Vector3 guideRendererLocalPosition = new Vector3(0f, 0f, 0.1f);

//    [Header("Advanced Model Target Guide View")]
//    [Tooltip("Ak je na ModelTargetBehaviour nastaveny Custom 2D Guide View Image, tento skript ho zobrazi prepnutim do GuideView2D.")]
//    public bool useGuideView2D = true;

//    [Tooltip("Male oneskorenie, aby sa Vuforia stihla inicializovat skor, ako zmenime GuideViewMode.")]
//    public float guideViewModeDelay = 0.15f;

//    [Tooltip("Ako dlho sa ma hladat objekt GuideViewRenderer po zapnuti Guide View.")]
//    public float guideRendererSearchTime = 2.0f;

//    private bool detectionStarted = false;
//    private bool isDetected = false;

//    private Coroutine guideViewModeCoroutine;
//    private Coroutine guideRendererCoroutine;
//    private Coroutine restartModelTargetsCoroutine;

//    void Start()
//    {
//        RequestGuideViewMode(ModelTargetBehaviour.GuideViewDisplayMode.NoGuideView);

//        if (detectedLogger != null)
//            detectedLogger.DisableDetection();

//        if (detectedUI != null)
//            detectedUI.DisableDetection();

//        if (detectionMenu != null)
//            detectionMenu.SetActive(true);

//        if (variablesUIRoot != null)
//            variablesUIRoot.SetActive(false);

//        Debug.Log("Advanced Model Target app initialized.");
//    }

//    public void StartDetection()
//    {
//        detectionStarted = true;
//        isDetected = false;

//        SetModelTargetsActive(true);

//        if (modelTargetBehaviour != null)
//        {
//            if (useGuideView2D)
//                RequestGuideViewMode(ModelTargetBehaviour.GuideViewDisplayMode.GuideView2D);
//            else
//                RequestGuideViewMode(ModelTargetBehaviour.GuideViewDisplayMode.NoGuideView);
//        }

//        if (detectedLogger != null)
//            detectedLogger.EnableDetection();

//        if (detectedUI != null)
//            detectedUI.EnableDetection();

//        if (detectionMenu != null)
//            detectionMenu.SetActive(true);

//        if (variablesUIRoot != null)
//            variablesUIRoot.SetActive(false);

//        if (guideRendererCoroutine != null)
//            StopCoroutine(guideRendererCoroutine);

//        guideRendererCoroutine = StartCoroutine(AdjustGuideRendererPositionRoutine());

//        Debug.Log("Detection started.");
//    }

//    IEnumerator AdjustGuideRendererPositionRoutine()
//    {
//        float elapsed = 0f;

//        while (elapsed < guideRendererSearchTime)
//        {
//            GameObject guideRenderer = GameObject.Find("GuideViewRenderer");

//            if (guideRenderer != null)
//            {
//                guideRenderer.transform.localPosition = guideRendererLocalPosition;
//                Debug.Log("GuideViewRenderer position upravena.");
//                yield break;
//            }

//            elapsed += 0.1f;
//            yield return new WaitForSeconds(0.1f);
//        }

//        Debug.LogWarning("GuideViewRenderer nebol najdeny.");
//    }

//    public void ConfirmDetectionSuccess()
//    {
//        if (!detectionStarted || isDetected)
//            return;

//        isDetected = true;

//        if (detectedLogger != null)
//            detectedLogger.DisableDetection();

//        if (detectedUI != null)
//            detectedUI.DisableDetection();

//        if (modelTargetBehaviour != null)
//            RequestGuideViewMode(ModelTargetBehaviour.GuideViewDisplayMode.NoGuideView);

//        if (variablesUIRoot != null)
//            variablesUIRoot.SetActive(true);

//        Debug.Log("Detection success confirmed. Variables UI shown.");
//    }

//    public void ToggleVariablesUI()
//    {
//        if (variablesUIRoot != null)
//            variablesUIRoot.SetActive(!variablesUIRoot.activeSelf);
//    }

//    public void ShowVariablesUI()
//    {
//        if (variablesUIRoot != null)
//            variablesUIRoot.SetActive(true);
//    }

//    public void HideVariablesUI()
//    {
//        if (variablesUIRoot != null)
//            variablesUIRoot.SetActive(false);
//    }

//    public void ResetDetectionState()
//    {
//        detectionStarted = false;
//        isDetected = false;

//        if (guideRendererCoroutine != null)
//        {
//            StopCoroutine(guideRendererCoroutine);
//            guideRendererCoroutine = null;
//        }

//        if (modelTargetBehaviour != null)
//            RequestGuideViewMode(ModelTargetBehaviour.GuideViewDisplayMode.NoGuideView);

//        if (detectedLogger != null)
//            detectedLogger.DisableDetection();

//        if (detectedUI != null)
//            detectedUI.DisableDetection();

//        if (detectionMenu != null)
//            detectionMenu.SetActive(true);

//        if (variablesUIRoot != null)
//            variablesUIRoot.SetActive(false);

//        if (restartModelTargetsCoroutine != null)
//            StopCoroutine(restartModelTargetsCoroutine);

//        restartModelTargetsCoroutine = StartCoroutine(RestartModelTargetsRoutine());

//        Debug.Log("Detection state reset. Model target objects will be restarted.");
//    }

//    private IEnumerator RestartModelTargetsRoutine()
//    {
//        SetModelTargetsActive(false);

//        yield return new WaitForSeconds(modelTargetRestartDelay);

//        SetModelTargetsActive(true);

//        Debug.Log("Model target objects restarted.");
//    }

//    private void SetModelTargetsActive(bool active)
//    {
//        if (modelTargetObjects == null || modelTargetObjects.Length == 0)
//        {
//            if (modelTargetBehaviour != null)
//                modelTargetBehaviour.gameObject.SetActive(active);

//            return;
//        }

//        foreach (GameObject targetObject in modelTargetObjects)
//        {
//            if (targetObject != null)
//                targetObject.SetActive(active);
//        }
//    }

//    private void RequestGuideViewMode(ModelTargetBehaviour.GuideViewDisplayMode mode)
//    {
//        if (guideViewModeCoroutine != null)
//            StopCoroutine(guideViewModeCoroutine);

//        guideViewModeCoroutine = StartCoroutine(SetGuideViewModeDelayed(mode));
//    }

//    private IEnumerator SetGuideViewModeDelayed(ModelTargetBehaviour.GuideViewDisplayMode mode)
//    {
//        yield return new WaitForSeconds(guideViewModeDelay);

//        if (modelTargetBehaviour == null)
//            yield break;

//        if (!modelTargetBehaviour.gameObject.activeInHierarchy)
//            yield break;

//        try
//        {
//            modelTargetBehaviour.GuideViewMode = mode;
//            Debug.Log($"GuideViewMode nastaveny na: {mode}");
//        }
//        catch (Exception ex)
//        {
//            Debug.LogWarning($"Nepodarilo sa nastavit GuideViewMode na {mode}. Dovod: {ex.Message}");
//        }
//    }
//}