using DashHA.Shared;

namespace DashHA.MqttService
{
    public interface IMqttService
    {
        Task<MqttStatusResponse> Publish_Application_Message(string message);
        Task<MqttStatusResponse> ConnectAsync();
        Task<MqttStatusResponse> DisconnectAsync();

        Task<MqttStatusResponse> AddTopicToSubscribeAsync(string topic);
        Task<MqttStatusResponse> RemoveTopicFromSubscribeAsync(string topic);
        event Func<MqttMessage, Task>? OnMessageReceived;
        List<MqttMessage> Messages { get; set; }
        //void AddMessage(MqttMessage message);


    }
}
