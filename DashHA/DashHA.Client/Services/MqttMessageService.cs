
using DashHA.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace DashHA.Client.Services
{
    public class MqttMessageService : IMqttMessageService
    {
        private readonly ILogger<MqttMessageService> _logger;
        private readonly NavigationManager _navigationManager;

        private HubConnection? _hubConnection;

        //register method when 
        public event Action<MqttMessage>? OnMessageReceived;
        //message history list
        public List<MqttMessage> Messages { get; set; } = new();


        public MqttMessageService(ILogger<MqttMessageService> logger, NavigationManager navigationManager)
        {
            _logger = logger;
            _navigationManager = navigationManager;


        }


        //feetch messages from server - update Messages list
        public async Task FetchMessagesAsync()
        {
            if (_hubConnection == null)
            {
                _logger.LogWarning("Messages is null. Upewnij się, że FetchMessagesAsync() zostało wywołane po StartAsync() i przed ConnectAsync().");
                return;
            }
            _logger.LogInformation("fetch messages");

            Messages = await _hubConnection.InvokeAsync<List<MqttMessage>>("GetMessages");
        }


        //start signalR connection
        public async Task StartAsync()
        {
            if (_hubConnection is not null) return;

            _logger.LogInformation("Starting MqttMessageService...");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/hubmqtt"))
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<MqttMessage>("ReceiveMessage", (message) =>
            {
                Messages.Add(message);
                OnMessageReceived?.Invoke(message);
            });

            await _hubConnection.StartAsync();

            //when connection is established, fetch messages
            //fetch messages from MqttService on server - lifetime of the app and assign to Messages on MqttMessageService on client 
            await FetchMessagesAsync();
        }


        public async Task<MqttStatusResponse> ConnectAsync()
        {
            if (_hubConnection is null)
            {
                _logger.LogWarning("Nie można połączyć – hubConnection jest null. Upewnij się, że StartAsync() zostało wywołane.");
                return new MqttStatusResponse(false, "HubConnection is not initialized");
            }

            try
            {
                var result = await _hubConnection.InvokeAsync<MqttStatusResponse>("Connect");
                _logger.LogInformation($"Połączono z MQTT przez SignalR. Odpowiedź: {result.MessageList.FirstOrDefault()}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wywoływania metody Connect przez SignalR");
                return new MqttStatusResponse(false, "Błąd połączenia przez SignalR");
            }
        }

        public async Task<MqttStatusResponse> SubscribeAsync(string topic)
        {
            if (_hubConnection is null)
                return new MqttStatusResponse(false, "Błąd połączenia przez SignalR");

            return await _hubConnection.InvokeAsync<MqttStatusResponse>("SubscribeTopic", topic);

        }

        public async Task<MqttStatusResponse> DisconnectAsync()
        {
            if (_hubConnection is null)
                return new MqttStatusResponse(false, "Błąd połączenia przez SignalR");

            return await _hubConnection.InvokeAsync<MqttStatusResponse>("Disconnect");
        }

        public async Task<MqttStatusResponse> SendMessageAsync(string message)
        {
            if (_hubConnection is null)
                return new MqttStatusResponse(false, "Błąd połączenia przez SignalR");

            return await _hubConnection.InvokeAsync<MqttStatusResponse>("SendMessage", message);

        }

        public async Task<MqttStatusResponse> UnsubscribeAsync(string topic)
        {
            if (_hubConnection is null)
                return new MqttStatusResponse(false, "Błąd połączenia przez SignalR");

            return await _hubConnection.InvokeAsync<MqttStatusResponse>("UnsubscribeTopic", topic);
        }

        public async Task<List<MqttMessage>> GetMqttMessagesAsync()
        {
            if (_hubConnection is null)
                return new List<MqttMessage>();

            return await _hubConnection.InvokeAsync<List<MqttMessage>>("GetMessages");

        }
    }
}
