using DashHA.Shared;

namespace DashHA.MqttNotificationService
{
    public interface IMqttNotificationService
    {

        event Func<MqttMessage, Task>? OnNotificationReceived;


    }
}
