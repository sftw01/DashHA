using DashHA.Shared;

namespace DashHA.Client.Services.NotificationService
{
    public class NotificationCardServiceClient : INotificationCardService
    {
        public event Action<MqttMessage> OnNotificationReceived;

        event Func<MqttMessage, Task> INotificationCardService.OnNotificationReceived
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
    }
}
