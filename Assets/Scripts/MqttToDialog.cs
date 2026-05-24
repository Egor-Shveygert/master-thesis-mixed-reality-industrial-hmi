using TMPro;
using UnityEngine;

public class MqttToDialogText : MonoBehaviour
{
    public PlcMqttClient mqttClient;
    public TMP_Text BooleanValue;
    public string topic = "hololens/status/test";
    public string prefix = "Value: ";

    private void OnEnable()
    {
        if (mqttClient != null)
            mqttClient.MessageArrived += OnMqttMessage;

        Refresh();
    }
    private void OnDisable()
    {
        if (mqttClient != null)
            mqttClient.MessageArrived -= OnMqttMessage;
    }
    private void OnMqttMessage(string incomingTopic, string payload)
    {
        if (incomingTopic != topic) return;
        if (BooleanValue == null) return;

        BooleanValue.text = prefix + payload;
        UpdateTextColor(payload);
    }
    private void Refresh()
    {
        if (mqttClient == null || BooleanValue == null) return;
        string value = mqttClient.GetLatestValue(topic, "--");
        BooleanValue.text = prefix + value;
        UpdateTextColor(value);
    }
    private void UpdateTextColor(string payload)
    {
        if (payload.ToLower() == "true")
        {
            BooleanValue.color = Color.green;
        }
        else if (payload.ToLower() == "false")
        {
            BooleanValue.color = Color.red;
        }
    }
}
