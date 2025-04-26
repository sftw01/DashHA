namespace DashHA.Shared
{
    public class MqttMessage
    {
        public string Topic { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;

        public MqttMessage() { }

        public MqttMessage(string topic, string payload)
        {
            Topic = topic;
            Payload = payload;
        }

        public override string ToString()
        {
            return $"Topic: {Topic}, Payload: {Payload}";
        }
    }
}
