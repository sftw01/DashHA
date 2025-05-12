using Microsoft.AspNetCore.SignalR;

namespace DashHA.Hubs
{
    public class HubNotificationCard : Hub
    {
        private readonly ILogger<HubNotificationCard> _logger;


        public HubNotificationCard(ILogger<HubNotificationCard> logger)
        {
            _logger = logger;
        }

        // Obsługa połączenia użytkownika
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Użytkownik z SignalR Id '{Context.ConnectionId}' połączony. - powiadomienie");
            await base.OnConnectedAsync();
        }

        // Obsługa rozłączenia użytkownika
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Użytkownik z SignalR Id '{Context.ConnectionId}' rozłączony. - powiadomienie");
            await base.OnDisconnectedAsync(exception);
        }
    }
}