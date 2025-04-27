using DashHA.Hubs;
using DashHA.Shared;
using Microsoft.AspNetCore.SignalR;
using MQTTnet;

namespace DashHA.MqttService
{
    public class MqttService : IMqttService
    {
        private readonly IMqttClient? _mqttClient;
        private readonly IHubContext<HubMqtt> _hubContext;
        private readonly ILogger<MqttService> _logger;

        //message history list
        public List<MqttMessage> Messages { get; set; } = new List<MqttMessage>();

        public event Func<MqttMessage, Task>? OnMessageReceived;


        //list of topics to subscribe
        private List<string> _topics = new List<string>();

        public MqttService(ILogger<MqttService> logger, IHubContext<HubMqtt> hubContext)
        {
            _logger = logger;

            var factory = new MqttClientFactory();
            _mqttClient = factory.CreateMqttClient();

            if (_mqttClient == null)
            {
                _logger.LogError("Nie można zainicjalizować klienta MQTT.");
                return;
            }

            // Subscribe application message received event
            _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedApplicationMessage;

            _logger.LogInformation("MqttService został zainicjalizowany.");
            _hubContext = hubContext;
        }

        public async Task<MqttStatusResponse> ConnectAsync()
        {
            if (_mqttClient == null)
            {
                _logger.LogError("Nie można połączyć się z brokerem MQTT. Klient jest null.");
                return new MqttStatusResponse(false, "Nie można połączyć się z brokerem MQTT. Klient jest null.");

            }

            if (_mqttClient.IsConnected)
            {
                _logger.LogWarning("MQTT client jest już połączony.");
                return new MqttStatusResponse(true, "MQTT client jest już połączony.");
            }

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com")
                //.WithTcpServer("localhost", 1883)
                .Build();

            try
            {
                var result = await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                if (result.ResultCode == MqttClientConnectResultCode.Success)
                {
                    _logger.LogInformation("Połączenie z brokerem MQTT zakończone sukcesem.");
                    return new MqttStatusResponse(true, "Połączenie z brokerem MQTT zakończone sukcesem.");


                }
                else
                {
                    _logger.LogError("Połączenie MQTT nie powiodło się: ResultCode: {ResultCode}", result.ResultCode);
                    return new MqttStatusResponse(false, $"Połączenie MQTT nie powiodło się: ResultCode {result.ResultCode}");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas próby połączenia z brokerem MQTT.");
                return new MqttStatusResponse(false, "Błąd podczas próby połączenia z brokerem MQTT.");
            }
        }

        public async Task<MqttStatusResponse> Publish_Application_Message(MqttMessage message)
        {

            if (_mqttClient?.IsConnected != true)
            {
                _logger.LogError("MQTT client nie jest połączony. Nie można opublikować wiadomości.");
                return new MqttStatusResponse(false, "MQTT client nie jest połączony. Nie można opublikować wiadomości.");
            }

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(message.Topic)
                .WithPayload(message.Payload)
                .Build();

            try
            {
                await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
                _logger.LogInformation("Wiadomość została opublikowana.");
                return new MqttStatusResponse(true, "Wiadomość została opublikowana.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas publikowania wiadomości MQTT.");
                return new MqttStatusResponse(false, "Błąd podczas publikowania wiadomości MQTT.");
            }
        }

        public async Task<MqttStatusResponse> DisconnectAsync()
        {
            //remove all currently subscribed topics from the lisyt
            _topics.Clear();

            if (_mqttClient == null)
            {
                _logger.LogWarning("Brak aktywnego polaczenia");
                return new MqttStatusResponse(false, "Brak aktywnego polaczenia");
            }

            if (!_mqttClient.IsConnected)
            {
                _logger.LogWarning("Juz Rozłączono");
                return new MqttStatusResponse(false, "Juz Rozłączono");
            }

            await _mqttClient.DisconnectAsync();
            _logger.LogInformation("Rozłączono");
            return new MqttStatusResponse(true, "Rozłączono");

        }
        //dispose
        public void Dispose()
        {
            if (_mqttClient != null)
            {
                _mqttClient.Dispose();
                _logger.LogInformation("MQTT client został zwolniony.");
            }
        }

        public async Task<MqttStatusResponse> AddTopicToSubscribeAsync(string topic)
        {
            if (_mqttClient == null)
            {
                _logger.LogError("Nie można subskrybować tematu. Klient jest null.");
                return new MqttStatusResponse(false, "Nie można subskrybować tematu. Klient jest null.");
            }

            if (!_mqttClient.IsConnected)
            {

                _logger.LogError("Nie można subskrybować tematu. Klient nie jest połączony.");
                return new MqttStatusResponse(false, "Nie można subskrybować tematu. Klient nie jest połączony.");
            }

            //check if topic is already subscribed - if yes return, otherwise add to list of topics currently subscribed
            if (_topics.Contains(topic))
            {
                _logger.LogWarning("Już subskrybujesz ten temat: {topic}", topic);
                return new MqttStatusResponse(false, $"Już subskrybujesz ten temat: {topic}");

            }

            var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(topic)
                .Build();
            try
            {
                var result = await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
                if (result.Items.Count > 0)
                {
                    _topics.Add(topic);

                    _logger.LogInformation("Subskrybowano temat: {topic}", topic);
                    return new MqttStatusResponse(true, $"Subskrybowano temat: {topic}");
                }
                else
                {
                    _logger.LogError("Nie można subskrybować tematu: {topic}", topic);
                    return new MqttStatusResponse(false, $"Nie można subskrybować tematu: {topic}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas subskrybowania tematu: {topic}", topic);
                return new MqttStatusResponse(false, $"Błąd podczas subskrybowania tematu: {topic}");
            }
        }

        public async Task<MqttStatusResponse> RemoveTopicFromSubscribeAsync(string topic)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                _logger.LogWarning("Klient MQTT nie jest połączony. Nie można odsubskrybować.");
                return new MqttStatusResponse(false, "Klient MQTT nie jest połączony. Nie można odsubskrybować.");
            }

            if (!_topics.Contains(topic))
            {
                _logger.LogWarning("Nie można usunąć tematu z subskrypcji, bo nie istnieje: {topic}", topic);
                return new MqttStatusResponse(false, $"Nie można usunąć tematu z subskrypcji, bo nie istnieje: {topic}");
            }

            try
            {
                var result = await _mqttClient.UnsubscribeAsync(topic, CancellationToken.None);

                if (result.Items.Any())
                {
                    _topics.Remove(topic);
                    _logger.LogInformation("Usunięto temat z listy subskrybowanych: {topic}", topic);
                    return new MqttStatusResponse(true, $"Odsubskrybowano temat: {topic}");
                }
                else
                {
                    _logger.LogWarning("Broker nie potwierdził odsubskrybowania tematu: {topic}", topic);
                    return new MqttStatusResponse(false, $"Broker nie potwierdził odsubskrybowania tematu: {topic}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas odsubskrybowania tematu: {topic}", topic);
                return new MqttStatusResponse(false, $"Błąd podczas odsubskrybowania tematu: {topic}");
            }
        }

        private async Task HandleReceivedApplicationMessage(MqttApplicationMessageReceivedEventArgs e)
        {

            var message = new MqttMessage
            {
                Topic = e.ApplicationMessage.Topic,
                Payload = e.ApplicationMessage.ConvertPayloadToString()
            };

            //add message to history list
            Messages.Add(message);

            if (_hubContext is not null)
            {
                _logger.LogInformation($"MqttService.cs - HandleReceived -  T: {message.Topic}, P: {message.Payload}");

                if (OnMessageReceived != null)
                {
                    await OnMessageReceived.Invoke(message);
                }
                else
                {
                    _logger.LogWarning("OnMessageReceived jest null");
                }
            }
        }

        public Task<List<MqttMessage>> GetMessagesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<MqttStatusResponse> ClearMessagesAsync()
        {
            throw new NotImplementedException();
        }
    }
}