using DashHA.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace DashHA.Client.Services.NotificationService
{
    // Implementing INotificationCardService interface
    public class NotificationCardServiceClient : INotificationCardService
    {
        private readonly ILogger<NotificationCardServiceClient> _logger;
        private readonly NavigationManager _navigationManager;
        private HubConnection? _hubConnection;

        public event Func<MqttMessage, Task>? OnNotificationReceived;

        public NotificationCardServiceClient(ILogger<NotificationCardServiceClient> logger, NavigationManager navigationManager)
        {
            _logger = logger;
            _navigationManager = navigationManager;
        }

        // Initialize SignalR connection
        public async Task InitializeAsync()
        {
            _logger.LogInformation("Starting SignalR client...");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/HubNotificationCard"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<MqttMessage>("OnNotification", (message) =>
            {
                _logger.LogInformation($"Received notification from SignalR: {message.Topic} - {message.Payload}");
                OnNotificationReceived?.Invoke(message);
            });

            await _hubConnection.StartAsync();
        }

        // Method to stop the connection
        public async Task StopAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _logger.LogInformation("SignalR connection stopped.");
            }
        }
    }
}