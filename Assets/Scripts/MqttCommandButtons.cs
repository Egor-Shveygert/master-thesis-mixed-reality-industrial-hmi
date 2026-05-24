using UnityEngine;

public class MqttCommandButtons : MonoBehaviour
{
    public PlcMqttClient mqttClient;

    public string statusTopic = "pibble/hololens/status/AutoModeActive";
    public string setTopic = "pibble/hololens/cmd/AutoModeActive/set";

    public void ToggleBool()
    {
        Debug.Log("[BTN] ToggleBool clicked");

        if (mqttClient == null)
        {
            Debug.LogError("[BTN] mqttClient is NULL");
            return;
        }

        var current = mqttClient.GetLatestValue(statusTopic, "false").Trim().ToLower();
        bool isTrue = (current == "true" || current == "1" || current == "on");
        bool next = !isTrue;

        Debug.Log($"[BTN] Publishing toggle -> {(next ? "true" : "false")} to {setTopic}");
        mqttClient.PublishString(setTopic, next ? "true" : "false");
    }

    public void SetTrue()
    {
        Debug.Log("[BTN] SetTrue clicked");

        if (mqttClient == null)
        {
            Debug.LogError("[BTN] mqttClient is NULL");
            return;
        }

        Debug.Log($"[BTN] Publishing true to {setTopic}");
        mqttClient.PublishString(setTopic, "true");
    }

    public void SetFalse()
    {
        Debug.Log("[BTN] SetFalse clicked");

        if (mqttClient == null)
        {
            Debug.LogError("[BTN] mqttClient is NULL");
            return;
        }

        Debug.Log($"[BTN] Publishing false to {setTopic}");
        mqttClient.PublishString(setTopic, "false");
    }
}