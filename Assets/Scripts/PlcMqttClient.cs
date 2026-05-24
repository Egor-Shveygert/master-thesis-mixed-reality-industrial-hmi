using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;

public class PlcMqttClient : M2MqttUnityClient
{
    [Header("MQTT Topics")]
    public string[] topics = new string[] { "pibble/hololens/status/test" };
    [Header("UI")]
    [SerializeField] private TMP_InputField brokerIpInput;
    [SerializeField] private TMP_Text currentIpText;
    [SerializeField] private TMP_Text connectionStatusText;
    [Header("MQTT Runtime Info")]
    [SerializeField] private string currentBrokerIp = "";
    [SerializeField] private int currentBrokerPort = 1883;

    public string lastTopic;
    public string lastMessage;
    public event Action<string, string> MessageArrived;
    private readonly Dictionary<string, string> latestValues = new Dictionary<string, string>();
    public string CurrentBrokerIp => currentBrokerIp;
    public int CurrentBrokerPort => currentBrokerPort;

    public string GetLatestValue(string topic, string fallback = "--")
    {
        return latestValues.TryGetValue(topic, out var value) ? value : fallback;
    }

    protected override void Start()
    {
        autoConnect = true;
        currentBrokerIp = brokerAddress;
        currentBrokerPort = brokerPort;

        base.Start();

        UpdateCurrentIpText($"MQTT IP: {brokerAddress}:{brokerPort} connecting...");
        UpdateConnectionStatus(false);

        if (brokerIpInput != null)
            brokerIpInput.text = brokerPort == 1883
                ? brokerAddress
                : $"{brokerAddress}:{brokerPort}";
    }

    private void UpdateConnectionStatus(bool isConnected)
    {
        if (connectionStatusText == null)
            return;

        if (isConnected)
        {
            connectionStatusText.text = "CONNECTED";
            connectionStatusText.color = Color.green;
        }
        else
        {
            connectionStatusText.text = "OFFLINE";
            connectionStatusText.color = Color.gray;
        }
    }

    public void ConnectToBrokerFromInput()
    {
        if (brokerIpInput == null)
        {
            Debug.LogError("Broker IP Input is not assigned.");
            UpdateCurrentIpText("MQTT IP: input missing");
            return;
        }

        string inputValue = brokerIpInput.text.Trim();

        if (string.IsNullOrWhiteSpace(inputValue))
        {
            Debug.LogWarning("Broker IP is empty.");
            UpdateCurrentIpText("MQTT IP: empty");
            return;
        }

        if (!TryParseBrokerInput(inputValue, out string parsedIp, out int parsedPort))
        {
            Debug.LogWarning("Invalid broker input: " + inputValue);
            UpdateCurrentIpText("MQTT IP: invalid");
            return;
        }

        ConnectToBroker(parsedIp, parsedPort);
    }

    public void ConnectToBroker(string ip, int port = 1883)
    {
        if (string.IsNullOrWhiteSpace(ip))
        {
            Debug.LogWarning("Cannot connect. Broker IP is empty.");
            UpdateCurrentIpText("MQTT IP: empty");
            return;
        }

        currentBrokerIp = ip;
        currentBrokerPort = port;
        brokerAddress = currentBrokerIp;
        brokerPort = currentBrokerPort;

        if (client != null && client.IsConnected)
        {
            Debug.Log("Disconnecting from previous MQTT broker...");
            Disconnect();
        }

        Debug.Log($"Connecting to MQTT broker {brokerAddress}:{brokerPort}...");
        UpdateCurrentIpText($"MQTT IP: {brokerAddress}:{brokerPort} connecting...");
        Connect();
    }

    private bool TryParseBrokerInput(string input, out string ip, out int port)
    {
        ip = "";
        port = 1883;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        input = input.Trim();
        string[] parts = input.Split(':');

        if (parts.Length == 1)
        {
            ip = parts[0].Trim();
            return !string.IsNullOrWhiteSpace(ip);
        }

        if (parts.Length == 2)
        {
            ip = parts[0].Trim();

            if (string.IsNullOrWhiteSpace(ip))
                return false;

            if (!int.TryParse(parts[1].Trim(), out port))
                return false;

            if (port <= 0 || port > 65535)
                return false;

            return true;
        }

        return false;
    }

    protected override void OnConnected()
    {
        base.OnConnected();
        Debug.Log($"MQTT connected to broker {brokerAddress}:{brokerPort}.");
        UpdateCurrentIpText($"MQTT IP: {brokerAddress}:{brokerPort}");
        UpdateConnectionStatus(true);
    }

    protected override void SubscribeTopics()
    {
        if (topics == null || topics.Length == 0)
            return;

        byte[] qos = new byte[topics.Length];

        for (int i = 0; i < qos.Length; i++)
            qos[i] = MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE;

        client.Subscribe(topics, qos);

        foreach (var topic in topics)
            Debug.Log("Subscribed to: " + topic);
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        if (topic == null || message == null)
        {
            Debug.LogWarning($"MQTT DecodeMessage: null topic or message (topic={topic ?? "null"})");
            return;
        }

        lastTopic = topic;
        lastMessage = Encoding.UTF8.GetString(message);
        latestValues[topic] = lastMessage;
        MessageArrived?.Invoke(topic, lastMessage);

        Debug.Log($"MQTT RX | Topic: {topic} | Payload: {lastMessage}");
    }

    public void PublishString(string topic, string payload)
    {
        if (client == null || !client.IsConnected)
        {
            Debug.LogWarning("MQTT client is not connected.");
            return;
        }

        client.Publish(
            topic,
            Encoding.UTF8.GetBytes(payload),
            MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE,
            false
        );

        Debug.Log($"MQTT TX | Topic: {topic} | Payload: {payload}");
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        Debug.LogError("MQTT connection failed: " + errorMessage);
        UpdateCurrentIpText($"MQTT IP: {brokerAddress}:{brokerPort} failed");
    }

    protected override void OnDisconnected()
    {
        Debug.Log("MQTT disconnected.");
        UpdateCurrentIpText($"MQTT IP: {brokerAddress}:{brokerPort} disconnected");
        UpdateConnectionStatus(false);
    }

    protected override void OnConnectionLost()
    {
        Debug.LogWarning("MQTT connection lost.");
        UpdateCurrentIpText($"MQTT IP: {brokerAddress}:{brokerPort} lost");
        UpdateConnectionStatus(false);
    }

    private void UpdateCurrentIpText(string value)
    {
        if (currentIpText != null)
            currentIpText.text = value;
    }

    protected virtual void OnDestroy()
    {
        Disconnect();
    }
}