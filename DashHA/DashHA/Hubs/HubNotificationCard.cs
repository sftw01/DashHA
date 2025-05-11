using DashHA.Client.Services.NotificationService;
using Microsoft.AspNetCore.SignalR;

namespace DashHA.Hubs
{
    public class HubNotificationCard : Hub<INotificationCardService>
    {
        //private readonly ILogger<HubNotificationCard> _logger;
        //private readonly IMqttNotificationService _mqttNotificationService;

        //public HubNotificationCard(IMqttNotificationService mqttNotificationService, ILogger<HubNotificationCard> logger)
        //{
        //    _mqttNotificationService = mqttNotificationService;
        //    _logger = logger;

        //    // Subskrybuj zdarzenie z poprawną metodą obsługi
        //    _mqttNotificationService.OnNotificationReceived += OnNotificationReceived;
        //}

        //// Metoda musi mieć parametr typu MqttMessage
        //public async Task OnNotificationReceived(MqttMessage message)
        //{
        //    _logger.LogInformation($"Otrzymano powiadomienie MQTT: {message.Topic} - {message.Payload}");
        //    // Przekazanie powiadomienia do klientów połączonych przez SignalR
        //    await Clients.All.OnNotificationReceived(message);
        //}

        //public override async Task OnConnectedAsync()
        //{
        //    Console.WriteLine($"Użytkownik z sigR Id '{Context.ConnectionId}' połączony. - powiadomienie");
        //}

        //public override async Task OnDisconnectedAsync(Exception? exception)
        //{
        //    Console.WriteLine($"Użytkownik z sigR Id '{Context.ConnectionId}' rozłączony. - powiadomienie");
        //}
    }
}
