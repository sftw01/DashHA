namespace DashHA.Shared
{
    public class MqttStatusResponse
    {
        public bool Success { get; set; }
        public List<string> MessageList { get; set; } = new();

        public MqttStatusResponse() { }


        public MqttStatusResponse(bool success)
        {
            Success = success;
        }

        public MqttStatusResponse(bool success, string message)
        {
            Success = success;
            MessageList.Add(message);
        }

        public MqttStatusResponse(bool success, List<string> messages)
        {
            Success = success;
            MessageList = messages;
        }
    }
}
