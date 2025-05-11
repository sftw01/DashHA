using DashHA.Shared;

namespace DashHA.Client.Services.NotificationService
{
    public interface INotificationCardService
    {

        //for client use - for notification service only!

        //tjis methoid will be invoked every time a notification is received
        event Func<MqttMessage, Task> OnNotificationReceived;

        Task InitializeAsync();


    }
}
