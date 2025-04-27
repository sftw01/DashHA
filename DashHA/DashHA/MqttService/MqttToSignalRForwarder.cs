using DashHA.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DashHA.MqttService
{
    public class MqttToSignalRForwarder
    {
        public MqttToSignalRForwarder(
            IMqttService mqttService,
            IHubContext<HubMqtt> hubContext,
            ILogger<MqttToSignalRForwarder> logger)
        {
            logger.LogInformation("👉 Forwarder wystartował");

            mqttService.OnMessageReceived += async (mqttMessage) =>
            {
                logger.LogInformation($"[FORWARDER] Otrzymano wiadomość MQTT: {mqttMessage.Topic} - {mqttMessage.Payload}");

                //add message toi history list 
                //mqttService.Messages.Add(mqttMessage);
                await hubContext.Clients.All.SendAsync("ReceiveMessage", mqttMessage);

                logger.LogInformation($"[FORWARDER] Wysłano przez SignalR: {mqttMessage.Topic}");
            };
        }
    }
}
