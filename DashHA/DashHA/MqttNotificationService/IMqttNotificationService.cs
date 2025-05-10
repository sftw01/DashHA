using DashHA.Shared;

namespace DashHA.MqttNotificationService
{
    public interface IMqttNotificationService
    {

        public event Func<MqttMessage, Task>? OnNotificationReceived;

    }
}
