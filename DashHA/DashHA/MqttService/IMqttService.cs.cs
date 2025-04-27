using DashHA.Shared;

namespace DashHA.MqttService
{
    public interface IMqttService
    {
        Task<MqttStatusResponse> Publish_Application_Message(MqttMessage message);
        Task<MqttStatusResponse> ConnectAsync();
        Task<MqttStatusResponse> DisconnectAsync();

        Task<MqttStatusResponse> AddTopicToSubscribeAsync(string topic);
        Task<MqttStatusResponse> RemoveTopicFromSubscribeAsync(string topic);

        //get all topics
        //Task<List<string>> GetAllTopicsAsync();

        event Func<MqttMessage, Task>? OnMessageReceived;
        List<MqttMessage> Messages { get; set; }

        Task<List<MqttMessage>> GetMessagesAsync();
        Task<MqttStatusResponse> ClearMessagesAsync();
        //void AddMessage(MqttMessage message);

    }
}
