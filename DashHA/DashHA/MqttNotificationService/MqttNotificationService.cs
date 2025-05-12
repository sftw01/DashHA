using DashHA.Hubs;
using DashHA.MqttService;
using DashHA.Shared;
using Microsoft.AspNetCore.SignalR;

namespace DashHA.MqttNotificationService
{
    public class MqttNotificationService : IMqttNotificationService
    {
        private readonly ILogger<MqttNotificationService> _logger;
        private readonly IMqttService _mqttService;
        private readonly IHubContext<HubNotificationCard> _hubContext;

        // Event for notifying about received MQTT messages (async)
        public event Func<MqttMessage, Task>? OnNotificationReceived;

        // List of active topics (events) to monitor
        private List<string> activeEvents = new List<string>();

        // Constructor with dependency injection (logger, MQTT service, and HubContext)
        public MqttNotificationService(
            ILogger<MqttNotificationService> logger,
            IMqttService mqttService,
            IHubContext<HubNotificationCard> hubContext)
        {
            _logger = logger;
            _mqttService = mqttService;
            _hubContext = hubContext;

            // Subscribing to the MQTT service message received event
            _mqttService.OnMessageReceived += HandleMqttMessageReceivedAsync;

            // Hardcoded topic for testing
            AddActiveEvent("sensor8");

            _logger.LogInformation($"MqttNotificationService started");
        }

        // Async method that handles incoming MQTT messages
        private async Task HandleMqttMessageReceivedAsync(MqttMessage message)
        {
            // Check if the message topic is in the list of active events
            if (activeEvents.Contains(message.Topic))
            {
                _logger.LogInformation($"Received MQTT event on topic: {message.Topic} payload: {message.Payload}");

                // Send the message to all SignalR clients
                await _hubContext.Clients.All.SendAsync("OnNotification", message);
                _logger.LogInformation($"Sent notification to SignalR clients: {message.Topic} - {message.Payload}");
            }
        }

        // Add a new active topic (event) to monitor
        public void AddActiveEvent(string topic)
        {
            if (!activeEvents.Contains(topic))
            {
                activeEvents.Add(topic);
                _logger.LogInformation($"Added active event for topic: {topic}");
            }
        }

        // Remove an active topic (event) from the list
        public void RemoveActiveEvent(string topic)
        {
            if (activeEvents.Contains(topic))
            {
                activeEvents.Remove(topic);
                _logger.LogInformation($"Removed active event for topic: {topic}");
            }
        }
    }
}