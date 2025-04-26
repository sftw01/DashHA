using DashHA.Shared;

namespace DashHA.Client.Services
{
    public interface IMqttMessageService
    {
        event Action<MqttMessage> OnMessageReceived;
        List<MqttMessage> Messages { get; }
        Task StartAsync();
        Task<MqttStatusResponse> ConnectAsync();
        Task<MqttStatusResponse> DisconnectAsync();
        Task<MqttStatusResponse> SendMessageAsync(MqttMessage message);
        Task<MqttStatusResponse> SubscribeAsync(string topic);
        Task<MqttStatusResponse> UnsubscribeAsync(string topic);

        Task<List<MqttMessage>> GetMqttMessagesAsync();   //storaged on MqttMessageService
        Task FetchMessagesAsync();  //from mqttService to MqttMessageService/MqttMessageServiceServer
        Task ClearMessageList(); //clear message list on MqttMessageService
        Task<List<MqttMessage>> GetMqttMessagesListAsync(); //get message list from MqttMessageServiceServer

    }
}
