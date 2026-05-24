//using UnityEngine;
//using Vuforia;

//public class DetectedModelLogger : MonoBehaviour
//{
//    private ObserverBehaviour observer;

//    [Header("Detection")]
//    public bool detectionEnabled = false;

//    [Header("App flow")]
//    //public AppStateController appStateController;
//    public AMTargetAppStateController appStateController;

//    private bool trackedAlreadyTriggered = false;

//    void Awake()
//    {
//        observer = GetComponent<ObserverBehaviour>();

//        if (observer != null)
//        {
//            observer.OnTargetStatusChanged += OnStatusChanged;
//        }
//    }

//    void OnDestroy()
//    {
//        if (observer != null)
//        {
//            observer.OnTargetStatusChanged -= OnStatusChanged;
//        }
//    }

//    public void EnableDetection()
//    {
//        detectionEnabled = true;
//        trackedAlreadyTriggered = false;
//        Debug.Log("Detection enabled");
//    }

//    public void DisableDetection()
//    {
//        detectionEnabled = false;
//        Debug.Log("Detection disabled");
//    }

//    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
//    {
//        if (!detectionEnabled)
//            return;

//        if (status.Status == Status.TRACKED ||
//            status.Status == Status.EXTENDED_TRACKED)
//        {
//            Debug.Log("Detected Model Target: " + behaviour.TargetName);

//            if (!trackedAlreadyTriggered)
//            {
//                trackedAlreadyTriggered = true;

//                if (appStateController != null)
//                {
//                    appStateController.ConfirmDetectionSuccess();
//                }
//                else
//                {
//                    Debug.LogWarning("AppStateController reference is missing in DetectedModelLogger.");
//                }
//            }
//        }
//        else if (status.Status == Status.NO_POSE)
//        {
//            Debug.Log("Model Target lost");
//        }
//    }
//}
