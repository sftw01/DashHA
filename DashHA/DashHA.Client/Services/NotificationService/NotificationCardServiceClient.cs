using DashHA.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace DashHA.Client.Services.NotificationService
{
    public class NotificationCardServiceClient : INotificationCardService
    {

        private readonly ILogger<NotificationCardServiceClient> _logger;
        private readonly NavigationManager _navigationManager;
        private HubConnection? _hubConnection;

        public event Func<MqttMessage, Task> OnNotificationReceived;


        public NotificationCardServiceClient(ILogger<NotificationCardServiceClient> logger, NavigationManager navigationManager)
        {
            _logger = logger;
            _navigationManager = navigationManager;

        }





    }
}
