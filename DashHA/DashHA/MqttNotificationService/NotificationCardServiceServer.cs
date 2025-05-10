using DashHA.Shared;

namespace DashHA.MqttNotificationService
{
    public class NotificationCardServiceServer : IMqttNotificationService
    {
        public event Func<MqttMessage, Task>? OnNotificationReceived;
    }
}
