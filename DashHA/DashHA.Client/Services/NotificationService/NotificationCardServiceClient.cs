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
            //if (_hubConnection is not null) return;


            //_logger.LogInformation("Inicjalizacja NotificationCardServiceClient...");

            //_hubConnection = new HubConnectionBuilder()
            //    .WithUrl(_navigationManager.ToAbsoluteUri("/hubnotificationcard"))
            //    .WithAutomaticReconnect()
            //    .Build();

            //_hubConnection.On<MqttMessage>("OnNotificationReceived", (message) =>
            //{
            //    _logger.LogInformation($"Otrzymano powiadomienie z SignalR: {message.Topic} - {message.Payload}");
            //    if (OnNotificationReceived != null)
            //    {
            //        OnNotificationReceived.Invoke(message);
            //    }
            //});


            //try
            //{
            //    await _hubConnection.StartAsync();
            //    _logger.LogInformation("Połączenie z SignalR zostało nawiązane.");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Błąd podczas nawiązywania połączenia SignalR: {ex.Message}");
            //}

            if (_hubConnection is not null) return;

            _logger.LogInformation("Starting MqttMessageService...");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/hubnotificationcard"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<MqttMessage>("ReceiveEvent !@!#!$", (message) =>
            {

                OnNotificationReceived?.Invoke(message);
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
