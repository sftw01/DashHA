using DashHA.Shared;

namespace DashHA.Client.Services
{
    public class NotificationCardServiceClient : INotificationCardService
    {
        public event Action<MqttMessage> OnNotificationReceived;
    }
}
