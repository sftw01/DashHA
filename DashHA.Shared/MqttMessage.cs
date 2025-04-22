namespace DashHA.Shared
{
    public class MqttMessage
    {
        public string Topic { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Topic: {Topic}, Payload: {Payload}";
        }
    }
}
