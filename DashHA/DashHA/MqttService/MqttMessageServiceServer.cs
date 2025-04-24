using DashHA.Client.Services;
using DashHA.Shared;

namespace DashHA.MqttService
{
    public class MqttMessageServiceServer : IMqttMessageService
    {

        private readonly ILogger<MqttMessageServiceServer> _logger;
        private readonly IMqttService _mqttService;

        //from interface
        //message history list
        public List<MqttMessage> Messages { get; set; } = new();


        public event Action<MqttMessage> OnMessageReceived;

        public MqttMessageServiceServer(IMqttService mqttService, ILogger<MqttMessageServiceServer> logger)
        {
            _mqttService = mqttService;
            _logger = logger;

            // Add event handler to mqttService
            _mqttService.OnMessageReceived += HandleMessageReceived;
        }

        // define a method to handle the OnMessageReceived event
        private async Task HandleMessageReceived(MqttMessage message)
        {
            _logger.LogInformation($"Message received: {message.Topic} - {message.Payload}");
            // Add the message to the local Messages list
            Messages.Add(message);
            // Invoke the OnMessageReceived event if there are any subscribers
            OnMessageReceived?.Invoke(message);
        }



        public Task<MqttStatusResponse> ConnectAsync() =>
            _mqttService.ConnectAsync();

        public Task<MqttStatusResponse> DisconnectAsync() =>
                       _mqttService.DisconnectAsync();

        public Task<List<MqttMessage>> GetMqttMessagesAsync() => Task.FromResult(_mqttService.Messages);

        public Task<MqttStatusResponse> SendMessageAsync(string message) =>
                       _mqttService.Publish_Application_Message(message);

        public async Task StartAsync()
        {
            //start signalR connection - not used on serverSide 

            //when connection is established, fetch messages
            //fetch messages from MqttService on server - lifetime of the app and assign to Messages on MqttMessageService on client 
            await FetchMessagesAsync();
        }

        public Task<MqttStatusResponse> SubscribeAsync(string topic) =>
                       _mqttService.AddTopicToSubscribeAsync(topic);

        public Task<MqttStatusResponse> UnsubscribeAsync(string topic) =>
                          _mqttService.RemoveTopicFromSubscribeAsync(topic);

        public Task FetchMessagesAsync()
        {
            Messages = _mqttService.Messages;
            return Task.CompletedTask;
        }

    }
}

