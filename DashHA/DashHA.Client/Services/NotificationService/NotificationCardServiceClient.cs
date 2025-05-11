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

            InitializeAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Inicjalizacja NotificationCardServiceClient.. <--------------------.");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/hubnotificationcard"))
                .Build();

            _hubConnection.On<MqttMessage>("OnNotificationReceived", async (message) =>
            {
                _logger.LogInformation($"Otrzymano powiadomienie z SignalR: {message.Topic} - {message.Payload}");
                if (OnNotificationReceived != null)
                    await OnNotificationReceived.Invoke(message);
            });

            await _hubConnection.StartAsync();
        }

        public async Task StopAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
