using DashHA.Client.Services.NotificationService;
using DashHA.Shared;

namespace DashHA.MqttNotificationService
{
    public class NotificationCardServiceServer : INotificationCardService
    {
        private readonly IMqttNotificationService _notificationService;
        private readonly ILogger<NotificationCardServiceServer> _logger;

        public event Func<MqttMessage, Task>? OnNotificationReceived;

        public NotificationCardServiceServer(IMqttNotificationService notificationService, ILogger<NotificationCardServiceServer> logger)
        {
            _notificationService = notificationService;
            _logger = logger;

            // handle notification when _notificationService receives any event
            _notificationService.OnNotificationReceived += RelayNotification;
        }

        // Method to relay the notification event
        private async Task RelayNotification(MqttMessage message)
        {
            _logger.LogInformation($"Received event {message.Topic}, {message.Payload}");
            if (OnNotificationReceived != null)
            {
                foreach (var handler in OnNotificationReceived.GetInvocationList())
                {
                    try
                    {
                        // Async invocation of event handlers
                        await ((Func<MqttMessage, Task>)handler)(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while forwarding notification");
                    }
                }
            }
        }
    }
}
