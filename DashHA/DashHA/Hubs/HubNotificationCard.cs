using DashHA.MqttNotificationService;
using DashHA.Shared;
using Microsoft.AspNetCore.SignalR;

namespace DashHA.Hubs
{
    public class HubNotificationCard : Hub
    {
        private readonly ILogger<HubNotificationCard> _logger;
        private readonly IMqttNotificationService _mqttNotificationService;

        public HubNotificationCard(IMqttNotificationService mqttNotificationService, ILogger<HubNotificationCard> logger)
        {
            _mqttNotificationService = mqttNotificationService;
            _logger = logger;

            // Subskrybuj zdarzenie z serwisu MQTT
            _mqttNotificationService.OnNotificationReceived += HandleMqttNotification;
        }

        // Obsługa powiadomienia MQTT
        private async Task HandleMqttNotification(MqttMessage message)
        {
            _logger.LogInformation($"Otrzymano powiadomienie MQTT tu kurwa: {message.Topic} - {message.Payload}");
            //await Clients.All()

            //OnNotificationReceived
            //    .SendAsync("OnNotificationReceived", message);

            if (_mqttNotificationService != null)
            {
                await Clients.All.SendAsync("OnNotification", message);

            }


        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Użytkownik z SignalR Id '{Context.ConnectionId}' połączony. - powiadomienie");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Użytkownik z SignalR Id '{Context.ConnectionId}' rozłączony. - powiadomienie");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
