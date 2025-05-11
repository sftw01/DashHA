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

            //InitializeAsync();
        }

        // Metoda inicjalizująca SignalR - ręcznie wywoływana
        public async Task InitializeAsync()
        {
            _logger.LogInformation("Starting MqttMessageService on client...");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/hubnotificationcard"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<MqttMessage>("OnNotification", (message) =>
            {

                OnNotificationReceived?.Invoke(message);
                Console.WriteLine($"Otrzymano powiadomienie z SignalR < ===== : {message.Topic} - {message.Payload}");
            });

            await _hubConnection.StartAsync();

        }


        // Metoda do zatrzymania połączenia
        public async Task StopAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
                _logger.LogInformation("Połączenie SignalR zostało zakończone.");
            }
        }
    }
}
