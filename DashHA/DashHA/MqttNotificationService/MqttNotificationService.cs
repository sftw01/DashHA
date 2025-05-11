using DashHA.MqttService;
using DashHA.Shared;

namespace DashHA.MqttNotificationService
{
    public class MqttNotificationService : IMqttNotificationService
    {
        private readonly ILogger<MqttNotificationService> _logger;
        private readonly IMqttService _mqttService;

        // Event for notifying about received MQTT messages (async)
        public event Func<MqttMessage, Task>? OnNotificationReceived;

        // List of active topics (events) to monitor
        private List<string> activeEvents = new List<string>();

        // Constructor with dependency injection (logger and MQTT service)
        public MqttNotificationService(ILogger<MqttNotificationService> logger, IMqttService mqttService)
        {
            _logger = logger;
            _mqttService = mqttService;

            // Subscribing to the MQTT service message received event
            //when mqttService receive any message
            _mqttService.OnMessageReceived += HandleMqttMessageReceivedAsync;


            //hardcoding
            AddActiveEvent("sensor8");

            _logger.LogInformation($" ----->>>> Received event class started");

        }

        // Async method that handles incoming MQTT messages
        private async Task HandleMqttMessageReceivedAsync(MqttMessage message)
        {
            // Check if the message topic is in the list of active events
            if (activeEvents.Contains(message.Topic))
            {
                _logger.LogInformation($"Received MQTT event on topic: {message.Topic} payload: {message.Payload}");

                // If there are subscribers, invoke the event asynchronously
                if (OnNotificationReceived != null)
                {
                    await OnNotificationReceived.Invoke(message);
                    _logger.LogInformation("OnReceiveNotificationEvent was invoked");
                }
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
