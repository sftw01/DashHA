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

        public event Func<MqttMessage, Task>? OnNotificationReceived;

        public NotificationCardServiceClient(ILogger<NotificationCardServiceClient> logger, NavigationManager navigationManager)
        {
            _logger = logger;
            _navigationManager = navigationManager;
        }

        // Metoda inicjalizująca SignalR - ręcznie wywoływana
        public async Task InitializeAsync()
        {
            if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
            {
                _logger.LogWarning("SignalR connection is already initialized.");
                return;
            }

            _logger.LogInformation("Starting MqttMessageService on client...");

            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_navigationManager.ToAbsoluteUri("/HubNotificationCard"))
                    .WithAutomaticReconnect()
                    .Build();

                _hubConnection.On<MqttMessage>("OnNotification", async (message) =>
                {
                    try
                    {
                        if (OnNotificationReceived is not null)
                        {
                            await OnNotificationReceived.Invoke(message);
                            _logger.LogInformation($"Otrzymano powiadomienie z SignalR < ===== : {message.Topic} - {message.Payload}");
                        }
                        else
                        {
                            _logger.LogWarning("No subscribers for OnNotificationReceived event.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while invoking OnNotificationReceived.");
                    }
                });

                await _hubConnection.StartAsync();
                _logger.LogInformation("SignalR connection established.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing SignalR connection.");
            }
        }

        // Metoda do zatrzymania połączenia
        public async Task StopAsync()
        {
            if (_hubConnection is null)
            {
                _logger.LogWarning("SignalR connection is not initialized.");
                return;
            }

            try
            {
                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    await _hubConnection.StopAsync();
                    _logger.LogInformation("Połączenie SignalR zostało zakończone.");
                }

                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                _logger.LogInformation("SignalR connection has been disposed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while stopping SignalR connection.");
            }
        }
    }
}