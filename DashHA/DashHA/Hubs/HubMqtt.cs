using DashHA.MqttService;
using DashHA.Shared;
using Microsoft.AspNetCore.SignalR;

namespace DashHA.Hubs
{
    public class HubMqtt : Hub
    {

        private readonly IMqttService _mqttService;
        private readonly ILogger<HubMqtt> _logger;



        public HubMqtt(IMqttService mqttService, ILogger<HubMqtt> logger)
        {
            _mqttService = mqttService;
            _logger = logger;

        }

        public Task<List<MqttMessage>> GetMessages()
        {
            return Task.FromResult(_mqttService.Messages);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"User with sigR Id '{Context.ConnectionId}' connected.");

            //simulate a delay
            await Task.Delay(10);

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"User with sigR Id '{Context.ConnectionId}' disconnected.");
            //simulate a delay

            await Task.Delay(10);

        }




        public async Task<MqttStatusResponse> SendMessage(MqttMessage message)
        {
            _logger.LogInformation($"User with sigR Id '{Context.ConnectionId}' sent message: {message}");
            //Console.WriteLine($"User with sigR Id '{Context.ConnectionId}' sent message: {message}");
            var result = await _mqttService.Publish_Application_Message(message);
            return result;

        }

        public async Task<MqttStatusResponse> Connect()
        {
            _logger.LogInformation($"User with sigR Id '{Context.ConnectionId}' connected to MQTT broker.");
            var result = await _mqttService.ConnectAsync();
            return result;

        }

        public async Task<MqttStatusResponse> Disconnect()
        {
            _logger.LogInformation($"User with sigR Id '{Context.ConnectionId}' disconnected from MQTT broker.");
            var result = await _mqttService.DisconnectAsync();
            return result;
        }


        public async Task<MqttStatusResponse> SubscribeTopic(string topic)
        {
            _logger.LogInformation($"User with sigR Id '{Context.ConnectionId}' subscribed to topic: {topic}");
            var result = await _mqttService.AddTopicToSubscribeAsync(topic);
            return result;
        }

        public async Task<MqttStatusResponse> UnsubscribeTopic(string topic)
        {
            _logger.LogInformation($"User with sigR Id '{Context.ConnectionId}' unsubscribed from topic: {topic}");
            var result = await _mqttService.RemoveTopicFromSubscribeAsync(topic);
            return result;
        }




    }
}